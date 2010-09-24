// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Magnum.ForNHibernate.Specs.Channels
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Concurrency;
	using Extensions;
	using Magnum.Channels;
	using NHibernate;
	using NUnit.Framework;
	using TestFramework;


	[TestFixture]
	[Explicit]
	public class Generating_a_large_volume_of_messages_to_entities :
		Given_an_empty_test_instance_table
	{
		[Test]
		public void Output_timing()
		{
			Trace.WriteLine("Execution Time: " + _stopwatch.ElapsedMilliseconds + "ms");
			Trace.WriteLine("Updates/Second: " + _count * 1000 / _stopwatch.ElapsedMilliseconds);
		}

		[Test]
		public void Should_have_a_single_row_for_each_value()
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				IList<TestInstance> list = session.CreateCriteria<TestInstance>().List<TestInstance>();

				int rowCount = list.Count;
				Trace.WriteLine("Rows stored in database: " + rowCount);

				int expectedCount = _updateValues.GroupBy(x => x.Id).Count();

				rowCount.ShouldEqual(expectedCount);


				var join = _updateValues.GroupBy(x => x.Id).Join(list, x => x.Key, x => x.Id, (g, l) => new
					{
						g.Key,
						Sum = g.Sum(v => v.Value),
						l.Value
					}).ToList();

				join.Count.ShouldEqual(expectedCount);

				join.Where(x => x.Sum == x.Value).Count().ShouldEqual(expectedCount);

			}
		}

		List<UpdateValue> _updateValues;
		Stopwatch _stopwatch;
		int _count;

		[TestFixtureSetUp]
		public void Should_perform_admirably()
		{
			var random = new Random();

			_count = 50000;

			_updateValues = Enumerable.Range(0, _count)
				.Select(x => random.Next(1, 1000))
				.Select(x => x > 960 ? 1000 : x)
				.Select(x => new UpdateValue(x, random.Next(1, 500000)/100m))
				.ToList();

			var input = new ChannelAdapter();
			using (input.Connect(x =>
				{
					x.AddConsumerOf<UpdateValue>()
						.UsingInstance()
						.Of<TestInstance>()
						.DistributedBy(msg => msg.Id)
						.PersistUsingNHibernate()
						.UsingSessionProvider(m => SessionFactory.OpenSession())
						.OnChannel(m => m.UpdateValueChannel)
						.CreateNewInstanceBy(m => new TestInstance(m.Id));
				}))
			{
				var complete = new Future<int>();
				var latch = new CountdownLatch(_count, complete.Complete);

				_stopwatch = Stopwatch.StartNew();
				UpdateValue.SetReceivedCallback(x => latch.CountDown());

				_updateValues.Each(input.Send);

				complete.WaitUntilCompleted(2.Minutes()).ShouldBeTrue();

				_stopwatch.Stop();
			}
		}
	}
}
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
namespace Magnum.Infrastructure.Specs.Channels
{
	using System.Linq;
	using Magnum.Channels;
	using NHibernate;
	using NUnit.Framework;
	using TestFramework;


	[TestFixture]
	public class Adding_nhibernate_channels
	{
		[Test]
		public void Should_support_single_channels()
		{
			var input = new ChannelAdapter();
			using (input.Connect(x =>
				{
					x.AddConsumerOf<UpdateValue>()
						.UsingInstance()
						.Of<TestInstance>()
						.DistributedBy(m => m.Id)
						.PersistUsingNHibernate()
						.UsingSessionProvider(m => (ISession)null)
						.OnChannel(m => m.UpdateValueChannel)
						.CreateNewInstanceBy(m => new TestInstance(m.Id));
				}))
			{
				input.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
					{
						typeof(ChannelAdapter),
						typeof(BroadcastChannel),
						typeof(TypedChannelAdapter<UpdateValue>),
						typeof(InstanceChannel<UpdateValue>),
					});
			}
		}
	}
}
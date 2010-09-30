// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Stact.Specs.Collections
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using NUnit.Framework;
	using TestFramework;


	[Scenario]
	[Explicit]
	public class Given_a_large_collection_of_objects
	{
		protected IList<Target> TargetList { get; private set; }
		protected int TargetCount { get; private set; }
		protected IDictionary<string, Target> TargetDictionary { get; private set; }

		[Given]
		public void A_large_collection_of_objects()
		{
			TargetCount = 1000000;

			TargetList = new List<Target>(TargetCount);
			TargetDictionary = new Dictionary<string, Target>(TargetCount);

			for (int i = 0; i < TargetCount; i++)
			{
				var target = new Target();

				TargetList.Add(target);
				TargetDictionary.Add(target.Name, target);
			}
		}


		protected class Target
		{
			static Random _random = new Random();

			public Target()
			{
				Id = CombGuid.Generate();
				Name = _random.GenerateRandomName();
			}

			public Guid Id { get; private set; }
			public string Name { get; set; }
		}
	}


	[Scenario]
	[Explicit]
	public class When_accessing_a_collection_using_an_index :
		Given_a_large_collection_of_objects
	{
		long _listTicks;
		long _dictionaryTicks;

		[When]
		public void Accessing_a_collection_using_an_index()
		{
			string key = TargetList[TargetList.Count - 1].Name;

			Target target = TargetList.Where(x => x.Name == key).Single();

			int iterations = 100;
			long totalTicks = 0L;

			for (int i = 0; i < iterations; i++)
			{
				var timer = Stopwatch.StartNew();
				target = TargetList.Where(x => x.Name == key).Single();
				timer.Stop();

				totalTicks = timer.ElapsedTicks;
			}

			_listTicks = totalTicks/iterations;

			totalTicks = 0L;

			for (int i = 0; i < iterations; i++)
			{
				var timer = Stopwatch.StartNew();
				target = TargetDictionary[key];
				timer.Stop();

				totalTicks = timer.ElapsedTicks;
			}

			_dictionaryTicks = totalTicks / iterations;


		}

		[Then]
		public void It_should_fast_for_large_object_counts()
		{
			Trace.WriteLine("List Time: " + _listTicks + " ticks");
			Trace.WriteLine("Dictionary Time: " + _dictionaryTicks + " ticks");

		}
	}
}
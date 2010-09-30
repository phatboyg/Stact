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
namespace Stact.Specs.Reflection
{
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using Stact.Reflection;
	using NUnit.Framework;

	[TestFixture]
	public class Comparing_the_performance_of_property_getters
	{
		private const int _count = 1000;
		private MyClass[] _objects;

		[TestFixtureSetUp]
		public void Setup()
		{
			_objects = new MyClass[_count];
			for (int i = 0; i < _count; i++)
			{
				_objects[i] = new MyClass {Value = i, Text = i.ToString()};
			}
		}

		private class MyClass
		{
			public int Value { get; set; }
			public string Text { get; set; }
		}

		[Test]
		public void Should_be_fast()
		{
			Trace.WriteLine("Refrection");

			Stopwatch timer = Stopwatch.StartNew();

			PropertyInfo valueProperty = typeof(MyClass).GetProperties().Where(x => x.Name == "Value").First();
			PropertyInfo textProperty = typeof (MyClass).GetProperties().Where(x => x.Name == "Text").First();

			for (int j = 0; j < 2; j++)
			{
				for (int i = 0; i < _count; i++)
				{
					object value = valueProperty.GetValue(_objects[i], null);
					object text = textProperty.GetValue(_objects[i], null);
				}
			}
			timer.Stop();

			Trace.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds);
		}

		[Test]
		public void Should_be_fast_or_faster()
		{
			Trace.WriteLine("FastProperty");

			Stopwatch timer = Stopwatch.StartNew();

			var valueProperty = typeof(MyClass).GetProperties().Where(x => x.Name == "Value").Select(x => new FastProperty(x)).First();
			var textProperty = typeof (MyClass).GetProperties().Where(x => x.Name == "Text").Select(x => new FastProperty(x)).First();

			for (int j = 0; j < 2; j++)
			{
				for (int i = 0; i < _count; i++)
				{
					object value = valueProperty.Get(_objects[i]);
					object text = textProperty.Get(_objects[i]);
				}
			}
			timer.Stop();

			Trace.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds);
		}
	}
}
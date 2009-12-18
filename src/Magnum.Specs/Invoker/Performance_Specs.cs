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
namespace Magnum.Specs.Invoker
{
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using Classes;
	using Magnum.Reflection;
	using NUnit.Framework;

	[TestFixture, Explicit]
	public class Performance_testing_of_the_invoker
	{
		[SetUp]
		public void Setup()
		{
			_stopwatch = Stopwatch.StartNew();
		}

		[TearDown]
		public void Teardown()
		{
			_stopwatch.Stop();

			Trace.WriteLine("Elapsed Time: " + _stopwatch.ElapsedMilliseconds + "ms");

			Trace.WriteLine("Invocations Per MS: " + _iterations / _stopwatch.ElapsedMilliseconds);
		}

		private Stopwatch _stopwatch;

		private const int _iterations = 300000;

		[Test]
		public void FirstTestName()
		{
			var target = new ClassWithGenericMethods();

			object name = "Name";
			object when = DateTime.Now;

			for (int i = 0; i < _iterations; i++)
			{
				target.FastInvoke(x => x.TwoGenericArguments(0, 0), name, when);
			}
		}

		[Test]
		public void Using_regular_old_Invoke()
		{
			var target = new ClassWithGenericMethods();

			object name = "Name";
			object when = DateTime.Now;

			var args = new object[] {name, when};

			MethodInfo method = typeof (ClassWithGenericMethods)
				.GetMethod("TwoGenericArguments");
				//.MakeGenericMethod(typeof (string), typeof (DateTime));

			for (int i = 0; i < _iterations; i++)
			{
				method.MakeGenericMethod(typeof (string), typeof (DateTime)).Invoke(target, args);
			}
		}
	}
}
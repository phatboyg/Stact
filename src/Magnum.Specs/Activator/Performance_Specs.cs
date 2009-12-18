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
namespace Magnum.Specs.Activator
{
	using System;
	using System.Diagnostics;
	using Classes;
	using Magnum.Reflection;
	using NUnit.Framework;

	[TestFixture, Explicit]
	public class Performance_testing_of_the_generator
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

			Trace.WriteLine("Allocations Per MS: " + Iterations/_stopwatch.ElapsedMilliseconds);
		}

		private const int Iterations = 5000000;
		private Stopwatch _stopwatch;

		[Test]
		public void Using_activator_with_no_arguments()
		{
			Trace.WriteLine("Using Activator.CreateInstance()");

			for (int i = 0; i < Iterations; i++)
			{
				var item = (ClassWithDefaultConstructor) Activator.CreateInstance(typeof (ClassWithDefaultConstructor));
			}
		}

		[Test]
		public void Using_activator_with_one_argument()
		{
			Trace.WriteLine("Using Activator.CreateInstance(47)");

			for (int i = 0; i < Iterations; i++)
			{
				var item = (ClassWithOneConstructorArg) Activator.CreateInstance(typeof (ClassWithOneConstructorArg), 47);
			}
		}

		[Test]
		public void Using_activator_with_two_arguments()
		{
			Trace.WriteLine("Using Activator.CreateInstance(47,TheName)");

			for (int i = 0; i < Iterations; i++)
			{
				var item = (ClassWithTwoConstructorArgs) Activator.CreateInstance(typeof (ClassWithTwoConstructorArgs), 47, "The Name");
			}
		}

		[Test]
		public void Using_new_with_no_arguments()
		{
			Trace.WriteLine("Using new()");

			for (int i = 0; i < Iterations; i++)
			{
				var item = new ClassWithDefaultConstructor();
			}
		}

		[Test]
		public void Using_new_with_one_argument()
		{
			Trace.WriteLine("Using new(47)");

			for (int i = 0; i < Iterations; i++)
			{
				var item = new ClassWithOneConstructorArg(47);
			}
		}

		[Test]
		public void Using_new_with_two_arguments()
		{
			Trace.WriteLine("Using new(47,TheName)");

			for (int i = 0; i < Iterations; i++)
			{
				var item = new ClassWithTwoConstructorArgs(47, "The Name");
			}
		}

		[Test]
		public void Using_object_generator_with_no_arguments()
		{
			Trace.WriteLine("Using FastActivator()");

			for (int i = 0; i < Iterations; i++)
			{
				ClassWithDefaultConstructor item = FastActivator<ClassWithDefaultConstructor>.Create();
			}
		}

		[Test]
		public void Using_object_generator_with_one_argument()
		{
			Trace.WriteLine("Using FastActivator(47)");

			for (int i = 0; i < Iterations; i++)
			{
				ClassWithOneConstructorArg item = FastActivator<ClassWithOneConstructorArg>.Create(47);
			}
		}

		[Test]
		public void Using_object_generator_with_two_arguments()
		{
			Trace.WriteLine("Using FastActivator(47,TheName)");

			for (int i = 0; i < Iterations; i++)
			{
				ClassWithTwoConstructorArgs item = FastActivator<ClassWithTwoConstructorArgs>.Create(47, "The Name");
			}
		}

		[Test]
		public void Using_object_generator_with_multiple_arguments()
		{
			Trace.WriteLine("Using FastActivator(47,TheName,123,Description)");

			var args = new object[] { 47, "The Name", 123, "Description" };
			for (int i = 0; i < Iterations; i++)
			{
				ClassWithMultipleConstructorArgs item = FastActivator<ClassWithMultipleConstructorArgs>.Create(args);
			}
		}

		[Test]
		public void Using_object_generator_via_type_and_arguments()
		{
			Trace.WriteLine("Using FastActivator(47,TheName) Via Type And Arguments");

			var args = new object[] {47, "The Name"};

			for (int i = 0; i < Iterations; i++)
			{
				var item = (ClassWithTwoConstructorArgs) FastActivator.Create(typeof (ClassWithTwoConstructorArgs), args);
			}
		}
	}
}
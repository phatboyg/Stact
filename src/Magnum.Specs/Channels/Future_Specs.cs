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
namespace Magnum.Specs.Channels
{
	using System;
	using NUnit.Framework;
	using TestFramework;


	[Scenario]
	public class Future_SpecsBase
	{
		public Future<object> Future { get; set; }

		[Given]
		public void A_future()
		{
			Future = new Future<object>();
		}
	}


	[Scenario]
	public class Completes_twice_same_objects :
		Future_SpecsBase
	{
		[When]
		public void Complete_is_called_twice_with_the_same_object()
		{
			var obj1 = new object();

			Future.Complete(obj1);
			Future.Complete(obj1);
		}

		[Then]
		public void No_exception_is_thrown()
		{
			Future.IsCompleted.ShouldBeTrue();
		}
	}


	[TestFixture]
	public class Completes_twice_different_objects
	{
		[Test]
		public void An_exception_is_thrown()
		{
			var future = new Future<object>();

			var obj1 = new object();
			var obj2 = new object();

			future.Complete(obj1);
			Assert.That(() => future.Complete(obj2), Throws.TypeOf<InvalidOperationException>());
		}
	}
}
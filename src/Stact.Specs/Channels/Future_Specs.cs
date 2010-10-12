// Copyright 2010 Chris Patterson
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
namespace Stact.Specs.Channels
{
	using System;
	using NUnit.Framework;
	using Magnum.TestFramework;


	[Scenario]
	public class Given_a_future_object
	{
		protected Future<object> Future { get; private set; }

		[Given]
		public void A_future()
		{
			Future = new Future<object>();
		}
	}


	[Scenario]
	public class When_complete_is_called :
		Given_a_future_object
	{
		[When]
		public void Complete_is_called()
		{
			var obj1 = new object();

			Future.Complete(obj1);
		}

		[Then]
		public void Should_have_been_completed()
		{
			Future.IsCompleted.ShouldBeTrue();
		}
	}


	[Scenario]
	public class When_complete_is_called_twice :
		Given_a_future_object
	{
		[Then]
		public void Should_throw_an_exception()
		{
			var obj1 = new object();
			var obj2 = new object();

			Future.Complete(obj1);
			Assert.That(() => Future.Complete(obj2), Throws.TypeOf<InvalidOperationException>());
		}
	}
}
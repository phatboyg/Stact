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
namespace Stact.Specs
{
	using Internal;
	using Magnum.TestFramework;
	using Routing;
	using Routing.Configuration;


	public class When_using_the_new_inbox
	{
		[Then]
		public void Should()
		{
			ActorFactory<MyActor> factory = ActorFactory.Create(inbox => new MyActor(inbox));






		}

		class A
		{ }
		class B
		{ }

		class MyActor :
			Actor
		{
			readonly Inbox _inbox;

			public MyActor(Inbox inbox)
			{
				_inbox = inbox;

				inbox.When<A>();

			}
		}
	}
}
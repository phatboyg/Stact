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
namespace Stact.Specs.Actors
{
	using Magnum.TestFramework;


	[Scenario]
	public class Using_an_actor
	{
		[Then]
		public void Should_not_require_extensive_namespace_references()
		{
			ActorInstance server = AnonymousActor.New(inbox =>
				{
					inbox.Receive<Request<MyRequest>>(request =>
						{
							return message =>
								{
									// send our response
									message.Respond(new MyResponse());
								};
						});
				});

			ActorInstance client = AnonymousActor.New(inbox =>
				{
					server.Request(new MyRequest(), inbox)
						.Receive<MyResponse>(response =>
							{
								return message =>
									{
										// do nothing
									};
							});
				});
		}


		class MyRequest
		{
		}


		class MyResponse
		{
		}
	}
}
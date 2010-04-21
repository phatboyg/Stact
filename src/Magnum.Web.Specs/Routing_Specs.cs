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
namespace Magnum.Web.Specs
{
	using System.Web.Routing;
	using Actors;
	using Channels;
	using Fibers;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Configuring_an_actor_with_one_channel
	{
		[Test]
		public void Should_properly_add_the_channel_to_the_route_collection()
		{
			var routes = new RouteCollection();

			routes.ConfigureActors(x =>
				{
					x.UseBasePath("a");

					x.Add<TestActor>().All();
				});

			routes.Count.ShouldEqual(1);

			routes.ShouldRoute("~/a/Test/RunTest", new {Actor = "Test", Channel = "RunTest"});
		}
	}

	public class TestInput
	{
	}

	public class TestActor
	{
		public TestActor()
		{
			RunTest = new ConsumerChannel<TestInput>(new SynchronousFiber(), x => { });
		}

		public Channel<TestInput> RunTest { get; set; }
	}
}
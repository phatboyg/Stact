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
namespace Stact.Specs.Usage
{
	using System.Diagnostics;
	using Magnum.Extensions;
	using Magnum.TestFramework;


	[Scenario]
	public class When_an_agent_is_paused_after_the_first_message
	{
		Update _first;
		Update _second;

		[When]
		public void An_agent_is_paused()
		{
			ActorFactory<Agent> agentFactory = ActorFactory.Create(inbox => new Agent(inbox));

			ActorRef agent = agentFactory.GetActor();

			_first = new Update();
			_second = new Update();

			agent.Send(_first);
			agent.Send<Suspend>();
			agent.Send(_second);

			_first.WaitUntilCompleted(2.Seconds());
			_second.WaitUntilCompleted(2.Seconds());
		}


		[Then]
		public void Should_process_the_first_message()
		{
			_first.IsCompleted.ShouldBeTrue();
		}

		[Then]
		public void Should_not_process_the_second_message()
		{
			_second.IsCompleted.ShouldBeFalse();
		}
	}


	[Scenario]
	public class When_an_agent_is_paused_and_later_continued
	{
		Update _first;
		Update _second;
		Update _third;

		[When]
		public void An_agent_is_paused()
		{
			ActorFactory<Agent> agentFactory = ActorFactory.Create(inbox => new Agent(inbox));

			ActorRef agent = agentFactory.GetActor();

			_first = new Update();
			_second = new Update();
			_third = new Update();

			agent.Send(_first);
			agent.Send<Suspend>();
			agent.Send(_second);

			_first.WaitUntilCompleted(2.Seconds());
			_second.WaitUntilCompleted(1.Seconds()).ShouldBeFalse();

			agent.Send<Resume>();
			_second.WaitUntilCompleted(2.Seconds());

			agent.Send(_third);
			_third.WaitUntilCompleted(2.Seconds());
		}


		[Then]
		public void Should_process_the_first_message()
		{
			_first.IsCompleted.ShouldBeTrue();
		}

		[Then]
		public void Should_process_the_second_message()
		{
			_second.IsCompleted.ShouldBeTrue();
		}

		[Then]
		public void Should_process_the_third_message()
		{
			_second.IsCompleted.ShouldBeTrue();
		}
	}


	class Agent :
		Actor
	{
		public Agent(Inbox inbox)
		{
			inbox.Loop(loop =>
			{
				loop.EnableSuspendResume(inbox)
					.Receive<Update>(update =>
					{
						update.Complete(Stopwatch.GetTimestamp());

						loop.Continue();
					});
			});
		}
	}


	class Update :
		Future<long> {}
}
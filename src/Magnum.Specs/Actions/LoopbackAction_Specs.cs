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
namespace Magnum.Specs.Actions
{
	using System.Collections.Generic;
	using System.Threading;
	using Magnum.Actions;
	using Magnum.Actors;
	using Magnum.Extensions;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class When_sending_actions_back_into_itself
	{
		[Test]
		public void Should_properly_release_one_waiting_writer()
		{
			const int writerCount = 10;
			const int messageCount = 100;

			var complete = new Future<bool>();
			int total = 0;

			ActionQueue reader = new ThreadPoolActionQueue(100, 60000);

			var writers = new List<ActionQueue>();
			for (int i = 0; i < writerCount; i++)
			{
				ActionQueue queue = new ThreadPoolActionQueue(100, 1000);
				for (int j = 0; j < messageCount; j++)
				{
					queue.Enqueue(() =>
						{
							Thread.Sleep(10);

							reader.Enqueue(() =>
								{
									total++;
									if (total == writerCount*messageCount)
										complete.Complete(true);
								});
						});
				}
			}

			complete.IsAvailable(10.Seconds()).ShouldBeTrue();
		}
	}
}
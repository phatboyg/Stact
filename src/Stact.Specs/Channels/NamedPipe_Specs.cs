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
	using System.IO.Pipes;
	using Magnum.TestFramework;


	[Scenario]
	public class Using_a_regular_named_pipe
	{
		[Then]
		public void Should_be_quicker_to_bootstrap_that_wcf()
		{
			using (var server = new NamedPipeServerStream("host", PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
			{
				using(var client = new NamedPipeClientStream(".", "host", PipeDirection.In, PipeOptions.Asynchronous))
				{

					client.Flush();
					client.Close();
				}

				server.Flush();
				server.Disconnect();
				server.Close();
			}
		}
	}
}
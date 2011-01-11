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
namespace Stact.Specs.Diagnostics
{
	using System.Diagnostics;


	public class TraceActor :
		Actor
	{
		public TraceActor(Inbox inbox)
		{
		}

		public void OperationStarted(Message<TraceOperationStarted> message)
		{
			Trace.WriteLine("Operation " + message.Body.Id + " started at " + message.Body.StartTime.ToShortTimeString());
		}

		public void OperationComplete(Message<TraceOperationComplete> message)
		{
			Trace.WriteLine("Operation " + message.Body.Id + " completed in " + message.Body.Duration + "ms");
		}

		public void OperationMessage(Message<TraceOperationMessage> message)
		{
			Trace.WriteLine("Operation " + message.Body.Id + " message at " + message.Body.Timestamp + ": " + message.Body.Message);
		}
	}
}
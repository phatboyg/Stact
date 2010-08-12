// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Magnum.Servers
{
	using StateMachine;


	public class SocketConnection :
		StateMachine<SocketConnection>
	{
		static SocketConnection()
		{
			Define(Definition);
		}

		public static State Initial { get; set; }
		public static State Completed { get; set; }
		public static State Connected { get; set; }
		public static State Disconnected { get; set; }

		public static Event ConnectionEstablished { get; set; }

		static void Definition()
		{
			Initially(
			          When(ConnectionEstablished)
			          	.Call(instance => instance.StartConnection())
			          	.TransitionTo(Connected)
				);
		}

		void StartConnection()
		{
		}
	}
}
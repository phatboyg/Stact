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
namespace Stact.Workflow
{
	public interface State :
		AcceptStateMachineVisitor
	{
		Event Enter { get; }
		Event Leave { get; }

		string Name { get; }
	}


	public interface State<TInstance> :
		State
	{
		void RaiseEvent(TInstance instance, Event e);
		void RaiseEvent<TBody>(TInstance instance, Event<TBody> e, TBody body);
	}
}
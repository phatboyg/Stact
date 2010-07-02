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
namespace Magnum.Visualizers.StateMachine
{
	using System;
	using System.IO;
	using System.Linq;
	using Magnum.StateMachine;
	using Microsoft.VisualStudio.DebuggerVisualizers;
	using Reflection;


	public class StateMachineVisualizerObjectSource :
		VisualizerObjectSource
	{
		public override void GetData(object target, Stream outgoingData)
		{
			if (target == null)
				return;

			Type targetType = target.GetType();

			if (!typeof(StateMachine).IsAssignableFrom(targetType))
				return;

			Type machineType = targetType.GetDeclaredGenericArguments().First();

			var args = new[] {target};

			StateMachineGraphData data =
				this.FastInvoke<StateMachineVisualizerObjectSource, StateMachineGraphData>(new[] {machineType},
				                                                                           "CreateStateMachineGraph", args);

			base.GetData(data, outgoingData);
		}

		StateMachineGraphData CreateStateMachineGraph<T>(T machine)
			where T : StateMachine<T>
		{
			var visitor = new GraphStateMachineVisitor<T>();
			machine.Inspect(visitor);

			return visitor.GetGraphData();
		}
	}
}
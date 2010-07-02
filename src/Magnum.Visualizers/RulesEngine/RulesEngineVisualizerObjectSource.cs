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
namespace Magnum.Visualizers.RulesEngine
{
	using System.IO;
	using Magnum.Pipeline;
	using Magnum.Pipeline.Visitors;
	using Magnum.RulesEngine;
	using Magnum.RulesEngine.ExecutionModel;
	using Magnum.StateMachine;
	using Microsoft.VisualStudio.DebuggerVisualizers;


	public class RulesEngineVisualizerObjectSource :
		VisualizerObjectSource
	{
		public override void GetData(object target, Stream outgoingData)
		{
			if (target == null)
				return;

			if (!typeof(RulesEngine).IsAssignableFrom(target.GetType()))
				return;

			var engine = (RulesEngine)target;

			RulesEngineGraphData data = engine.GetGraphData();

			base.GetData(data, outgoingData);
		}
	}

	public class PipelineVisualizerObjectSource :
		VisualizerObjectSource
	{
		public override void GetData(object target, Stream outgoingData)
		{
			if (target == null)
				return;

			if (!typeof(Pipe).IsAssignableFrom(target.GetType()))
				return;

			var pipe = (Pipe)target;

			PipelineGraphData data = pipe.GetGraphData();

			base.GetData(data, outgoingData);
		}
	}
}
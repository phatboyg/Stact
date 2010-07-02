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
using System.Diagnostics;
using Magnum.Channels;
using Magnum.Pipeline.Segments;
using Magnum.RulesEngine;
using Magnum.StateMachine;
using Magnum.Visualizers.Channels;
using Magnum.Visualizers.Pipeline;
using Magnum.Visualizers.RulesEngine;
using Magnum.Visualizers.StateMachine;
using Microsoft.VisualStudio.DebuggerVisualizers;

[assembly: DebuggerVisualizer(typeof(RulesEngineDebugVisualizer), typeof(VisualizerObjectSource),
	Target = typeof(MagnumRulesEngine),
	Description = "Rules Engine Graph Visualizer")]
[assembly: DebuggerVisualizer(typeof(PipelineDebugVisualizer), typeof(VisualizerObjectSource),
	Description = "Pipeline Graph Visualizer",
	Target = typeof(InputSegment))]
[assembly: DebuggerVisualizer(typeof(ChannelDebugVisualizer), typeof(ChannelVisualizerObjectSource),
	Description = "Channel Visualizer",
	Target = typeof(ChannelAdapter))]
[assembly: DebuggerVisualizer(typeof(StateMachineDebugVisualizer), typeof(StateMachineVisualizerObjectSource),
	Description = "State Machine Visualizer",
	Target = typeof(StateMachine<>))]
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
using Magnum.Pipeline;
using Magnum.RulesEngine;
using Magnum.RulesEngine.Visualizers;
using Microsoft.VisualStudio.DebuggerVisualizers;

[assembly : DebuggerVisualizer(typeof (RulesEngineDebugVisualizer),
	typeof (VisualizerObjectSource), 
	Target = typeof (RulesEngine),
	Description = "Rules Engine Graph Visualizer")]

[assembly: DebuggerVisualizer(typeof(PipelineDebugVisualizer),
	typeof(VisualizerObjectSource),
	Description = "Pipeline Graph Visualizer",
	Target = typeof(Pipe))]
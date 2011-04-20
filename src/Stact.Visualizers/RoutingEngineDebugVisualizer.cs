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
namespace Stact.Visualizers
{
	using System;
	using System.Windows.Forms;
	using Microsoft.Glee.Drawing;
	using Microsoft.VisualStudio.DebuggerVisualizers;
	using Routing;
	using Routing.Visualizers;


	[Serializable]
	public class RoutingEngineDebugVisualizer :
		DialogDebuggerVisualizer
	{
		protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
		{
			try
			{
				var data = (RoutingEngineGraphData)objectProvider.GetObject();

				Graph graph = new RoutingEngineGraphGenerator().CreateGraph(data);

				using (var form = new GraphVisualizerForm(graph, "Stact Routing Engine Visualizer"))
					windowService.ShowDialog(form);
			}
			catch (InvalidCastException)
			{
				MessageBox.Show("The selected data is not of a type compatible with this visualizer.",
				                GetType().ToString());
			}
		}

		public static void Show(RoutingEngine engine)
		{
			RoutingEngineGraphData graphData = engine.GetGraphData();
			Show(graphData);
		}

		public static void Show(RoutingEngineGraphData data)
		{
			var visualizerHost = new VisualizerDevelopmentHost(data, typeof(RoutingEngineDebugVisualizer));
			visualizerHost.ShowVisualizer();
		}
	}
}
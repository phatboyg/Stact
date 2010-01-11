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
namespace Magnum.RulesEngine.Visualizers
{
	using System.Diagnostics;
	using Microsoft.VisualStudio.DebuggerVisualizers;

	public class RulesEngineDebugVisualizer :
		DialogDebuggerVisualizer
	{
		protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
		{
			var data = objectProvider.GetObject() as RulesEngine;
			if (data == null)
			{
				Trace.WriteLine("Unable to display rules engine visualizer");
				return;
			}

			using (var form = new RulesEngineVisualizerForm(data))
			{
				windowService.ShowDialog(form);
			}
		}

		public static void TestShowVisualizer(RulesEngine rulesEngine)
		{
			var visualizerHost = new VisualizerDevelopmentHost(rulesEngine, typeof (RulesEngineDebugVisualizer));
			visualizerHost.ShowVisualizer();
		}
	}
}
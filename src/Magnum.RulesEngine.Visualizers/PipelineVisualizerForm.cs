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
	using System;
	using System.Windows.Forms;
	using Microsoft.Glee.Drawing;
	using Microsoft.Glee.GraphViewerGdi;
	using Pipeline;

	public partial class PipelineVisualizerForm : Form
	{
		private readonly Pipe _pipeline;

		public PipelineVisualizerForm(Pipe pipeline)
		{
			_pipeline = pipeline;
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			GViewer viewer = CreateGraphControl();

			Controls.Add(viewer);
		}

		private GViewer CreateGraphControl()
		{
			var viewer = new GViewer();
			viewer.Dock = DockStyle.Fill;
			viewer.Graph = CreateGraphForObject();
			return viewer;
		}

		private Graph CreateGraphForObject()
		{
			return new PipelineGraphGenerator().CreateGraph(_pipeline);
		}

		private void RulesEngineVisualizerForm_Load(object sender, EventArgs e)
		{
		}
	}
}
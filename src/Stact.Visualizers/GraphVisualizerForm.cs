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
namespace Stact.Visualizers
{
	using System;
	using System.Windows.Forms;
	using Microsoft.Glee.Drawing;
	using Microsoft.Glee.GraphViewerGdi;


	public partial class GraphVisualizerForm :
		Form
	{
		readonly Graph _graph;
		readonly string _caption;

		public GraphVisualizerForm(Graph graph, string caption)
		{
			_graph = graph;
			_caption = caption;

			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Text = _caption;

			GViewer viewer = CreateGraphControl();

			Controls.Add(viewer);
		}

		GViewer CreateGraphControl()
		{
			var viewer = new GViewer
				{
					Dock = DockStyle.Fill,
					Graph = _graph
				};

			return viewer;
		}
	}
}
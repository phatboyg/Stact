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
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Imaging;
	using Magnum.RulesEngine;
	using Magnum.RulesEngine.ExecutionModel;
	using Magnum.Extensions;
	using Magnum.Graphing;
	using Microsoft.Glee.Drawing;
	using Microsoft.Glee.GraphViewerGdi;
	using QuickGraph;
	using QuickGraph.Glee;


	public class RulesEngineGraphGenerator
	{
		private static Dictionary<Type, Microsoft.Glee.Drawing.Color> _colors;

		static RulesEngineGraphGenerator()
		{
			_colors = new Dictionary<Type, Microsoft.Glee.Drawing.Color>
				{
					{typeof (AlphaNode<>), Microsoft.Glee.Drawing.Color.Red},
					{typeof (TypeNode<>), Microsoft.Glee.Drawing.Color.Orange},
					{typeof (JoinNode<>), Microsoft.Glee.Drawing.Color.Green},
					{typeof (ConditionNode<>), Microsoft.Glee.Drawing.Color.Blue},
					{typeof (ActionNode<>), Microsoft.Glee.Drawing.Color.Teal},
					{typeof (ConstantNode<>), Microsoft.Glee.Drawing.Color.Magenta},
				};
		}

		public void SaveGraphToFile(RulesEngineGraphData data, int width, int height, string filename)
		{
			Graph gleeGraph = CreateGraph(data);

			var renderer = new GraphRenderer(gleeGraph);
			renderer.CalculateLayout();

			var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			renderer.Render(bitmap);

			Trace.WriteLine("Saving graph to " + filename);

			bitmap.Save(filename, ImageFormat.Png);
		}


		public Graph CreateGraph(RulesEngineGraphData data)
		{
			var graph = new AdjacencyGraph<Vertex, Edge<Vertex>>();

			data.Vertices.Each(x => graph.AddVertex(x));
			data.Edges.Each(x => graph.AddEdge(new Edge<Vertex>(x.From, x.To)));

			GleeGraphPopulator<Vertex, Edge<Vertex>> glee = graph.CreateGleePopulator();

			glee.NodeAdded += NodeStyler;
			glee.EdgeAdded += EdgeStyler;
			glee.Compute();

			Graph gleeGraph = glee.GleeGraph;

			return gleeGraph;
		}


		private void NodeStyler(object sender, GleeVertexEventArgs<Vertex> args)
		{
			Microsoft.Glee.Drawing.Color color = GetVertexColor(args.Vertex.VertexType);

			args.Node.Attr.Fillcolor = color;

			args.Node.Attr.Shape = Shape.Box;
			args.Node.Attr.Fontcolor = Microsoft.Glee.Drawing.Color.White;
			args.Node.Attr.Fontsize = 8;
			args.Node.Attr.FontName = "Arial";
			args.Node.Attr.Label = args.Vertex.Title;
			args.Node.Attr.Padding = 1.2;
		}


		private static Microsoft.Glee.Drawing.Color GetVertexColor(Type type)
		{
			return _colors.Retrieve(type, () => Microsoft.Glee.Drawing.Color.Black);
		}


		private static void EdgeStyler(object sender, GleeEdgeEventArgs<Vertex, Edge<Vertex>> e)
		{
			e.GEdge.EdgeAttr.Label = e.Edge.Source.TargetType.Name;
			e.GEdge.EdgeAttr.FontName = "Tahoma";
			e.GEdge.EdgeAttr.Fontsize = 6;
		}
	}
}
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
	using System.Drawing;
	using System.Drawing.Imaging;
	using Magnum.Collections;
	using Magnum.Extensions;
	using Magnum.Graphing;
	using Microsoft.Glee.Drawing;
	using Microsoft.Glee.GraphViewerGdi;
	using QuickGraph;
	using QuickGraph.Glee;
	using Routing.Internal;
	using Routing.Visualizers;


	public class RoutingEngineGraphGenerator
	{
		readonly Cache<Type, Microsoft.Glee.Drawing.Color> _colors;

		public RoutingEngineGraphGenerator()
		{
			_colors = new Cache<Type, Microsoft.Glee.Drawing.Color>
			{
				{typeof(RootNode), Microsoft.Glee.Drawing.Color.Black},
				{typeof(AlphaNode<>), Microsoft.Glee.Drawing.Color.Red},
				{typeof(ConditionNode<,>), Microsoft.Glee.Drawing.Color.Blue},
				{typeof(ConstantNode<>), Microsoft.Glee.Drawing.Color.Magenta},
				{typeof(JoinNode<>), Microsoft.Glee.Drawing.Color.Green},
				{typeof(JoinNode<,>), Microsoft.Glee.Drawing.Color.Green},
				{typeof(BodyNode<>), Microsoft.Glee.Drawing.Color.Cyan},
				{typeof(BodyNode<,,,>), Microsoft.Glee.Drawing.Color.Cyan},
				{typeof(ConsumerNode<>), Microsoft.Glee.Drawing.Color.Black},
				{typeof(SelectiveConsumerNode<>), Microsoft.Glee.Drawing.Color.Black},
			};
		}

		public void SaveGraphToFile(RoutingEngineGraphData data, int width, int height, string filename)
		{
			Graph gleeGraph = CreateGraph(data);

			var renderer = new GraphRenderer(gleeGraph);
			renderer.CalculateLayout();

			var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			renderer.Render(bitmap);

			bitmap.Save(filename, ImageFormat.Png);
		}


		public Graph CreateGraph(RoutingEngineGraphData data)
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


		void NodeStyler(object sender, GleeVertexEventArgs<Vertex> args)
		{
			Microsoft.Glee.Drawing.Color color = GetVertexColor(args.Vertex.VertexType);

			args.Node.Attr.Fillcolor = color;

			args.Node.Attr.Shape = Shape.Box;
			args.Node.Attr.Fontcolor = Microsoft.Glee.Drawing.Color.White;
			args.Node.Attr.Fontsize = 6;
			args.Node.Attr.FontName = "Arial";
			args.Node.Attr.Label = args.Vertex.Title;
			args.Node.Attr.Padding = 1.2;
		}


		Microsoft.Glee.Drawing.Color GetVertexColor(Type type)
		{
			return _colors.Retrieve(type, _ => Microsoft.Glee.Drawing.Color.Black);
		}

		static void EdgeStyler(object sender, GleeEdgeEventArgs<Vertex, Edge<Vertex>> e)
		{
			e.GEdge.EdgeAttr.Label = e.Edge.Source.TargetType != null ? e.Edge.Source.TargetType.ToShortTypeName() : "";
			e.GEdge.EdgeAttr.FontName = "Tahoma";
			e.GEdge.EdgeAttr.Fontsize = 6;
		}
	}
}
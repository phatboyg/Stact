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
namespace Magnum.Visualizers.Pipeline
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Imaging;
	using Extensions;
	using Graphing;
	using Magnum.Pipeline;
	using Magnum.Pipeline.Segments;
	using Magnum.Pipeline.Visitors;
	using Microsoft.Glee.Drawing;
	using Microsoft.Glee.GraphViewerGdi;
	using QuickGraph;
	using QuickGraph.Glee;


	public class PipelineGraphGenerator :
		GraphGenerator
	{
		static Dictionary<Type, Microsoft.Glee.Drawing.Color> _colors;

		static PipelineGraphGenerator()
		{
			_colors = new Dictionary<Type, Microsoft.Glee.Drawing.Color>
				{
					{typeof(InputSegment), Microsoft.Glee.Drawing.Color.Green},
					{typeof(FilterSegment), Microsoft.Glee.Drawing.Color.Yellow},
					{typeof(RecipientListSegment), Microsoft.Glee.Drawing.Color.Orange},
					{typeof(EndSegment), Microsoft.Glee.Drawing.Color.Red},
					{typeof(MessageConsumerSegment), Microsoft.Glee.Drawing.Color.Blue},
				};
		}

		public Graph CreateGraph(IEnumerable<Vertex> vertices, IEnumerable<Graphing.Edge> edges)
		{
			var graph = new AdjacencyGraph<Vertex, Edge<Vertex>>();

			vertices.Each(x => graph.AddVertex(x));
			edges.Each(x => graph.AddEdge(new Edge<Vertex>(x.From, x.To)));

			GleeGraphPopulator<Vertex, Edge<Vertex>> glee = graph.CreateGleePopulator();

			glee.NodeAdded += NodeStyler;
			glee.EdgeAdded += EdgeStyler;
			glee.Compute();

			Graph gleeGraph = glee.GleeGraph;

			return gleeGraph;
		}

		public void SaveGraphToFile(Pipe pipe, int width, int height, string filename)
		{
			Graph gleeGraph = CreateGraph(pipe);

			var renderer = new GraphRenderer(gleeGraph);
			renderer.CalculateLayout();

			var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			renderer.Render(bitmap);

			Trace.WriteLine("Saving graph to " + filename);

			bitmap.Save(filename, ImageFormat.Png);
		}

		public Graph CreateGraph(Pipe pipe)
		{
			var visitor = new GraphPipelineVisitor();
			visitor.Visit(pipe);

			return CreateGraph(visitor.Vertices, visitor.Edges);
		}

		void NodeStyler(object sender, GleeVertexEventArgs<Vertex> args)
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

		static Microsoft.Glee.Drawing.Color GetVertexColor(Type type)
		{
			return _colors.Retrieve(type, () => Microsoft.Glee.Drawing.Color.Black);
		}

		static void EdgeStyler(object sender, GleeEdgeEventArgs<Vertex, Edge<Vertex>> e)
		{
			e.GEdge.EdgeAttr.Label = e.Edge.Source.TargetType.Name;
			e.GEdge.EdgeAttr.FontName = "Tahoma";
			e.GEdge.EdgeAttr.Fontsize = 6;
		}
	}
}
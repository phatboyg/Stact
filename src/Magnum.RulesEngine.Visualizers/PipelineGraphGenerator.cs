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
	using System.Drawing;
	using System.Drawing.Imaging;
	using Microsoft.Glee.Drawing;
	using Microsoft.Glee.GraphViewerGdi;
	using Pipeline;
	using QuickGraph;
	using QuickGraph.Glee;
	using Color=Microsoft.Glee.Drawing.Color;

	public class PipelineGraphGenerator
	{
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
			var visitor = new PipelineGraphVisitor();
			visitor.Visit(pipe);

			var graph = new AdjacencyGraph<Vertex, Edge<Vertex>>();

			visitor.Vertices.Each(x => graph.AddVertex(x));
			visitor.Edges.Each(x => graph.AddEdge(new Edge<Vertex>(x.From, x.To)));

			GleeGraphPopulator<Vertex, Edge<Vertex>> glee = graph.CreateGleePopulator();

			glee.NodeAdded += NodeStyler;
			glee.EdgeAdded += EdgeStyler;
			glee.Compute();

			Graph gleeGraph = glee.GleeGraph;

			return gleeGraph;
		}

		private void NodeStyler(object sender, GleeVertexEventArgs<Vertex> args)
		{
			var color = new Color(args.Vertex.Color.R, args.Vertex.Color.G, args.Vertex.Color.B);

			args.Node.Attr.Fillcolor = color;

			args.Node.Attr.Shape = Shape.Box;
			args.Node.Attr.Fontcolor = Color.White;
			args.Node.Attr.Fontsize = 8;
			args.Node.Attr.FontName = "Arial";
			args.Node.Attr.Label = args.Vertex.Title;
			args.Node.Attr.Padding = 1.2;
		}

		private static void EdgeStyler(object sender, GleeEdgeEventArgs<Vertex, Edge<Vertex>> e)
		{
			e.GEdge.EdgeAttr.Label = e.Edge.Source.ObjectType.Name;
			e.GEdge.EdgeAttr.FontName = "Tahoma";
			e.GEdge.EdgeAttr.Fontsize = 6;
		}
	}
}
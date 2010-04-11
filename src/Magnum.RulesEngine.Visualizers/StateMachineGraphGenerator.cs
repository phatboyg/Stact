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
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Linq;
	using Extensions;
	using Graphing;
	using Microsoft.Glee.Drawing;
	using Microsoft.Glee.GraphViewerGdi;
	using QuickGraph;
	using QuickGraph.Glee;
	using Reflection;
	using StateMachine;

	public class StateMachineGraphGenerator :
		GraphGenerator
	{
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

		public void SaveGraphToFile(StateMachine machine, int width, int height, string filename)
		{
			Graph gleeGraph = CreateGraph(machine);

			var renderer = new GraphRenderer(gleeGraph);
			renderer.CalculateLayout();

			var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			renderer.Render(bitmap);

			Trace.WriteLine("Saving graph to " + filename);

			bitmap.Save(filename, ImageFormat.Png);
		}

		public Graph CreateGraph(StateMachine machine)
		{
			var args = new object[] {machine};

			Type machineType = machine.GetType().GetDeclaredGenericArguments().First();

			return this.FastInvoke<StateMachineGraphGenerator, Graph>(new[] {machineType}, "CreateStateMachineGraph", args);
		}

		private Graph CreateStateMachineGraph<T>(T machine)
			where T : StateMachine<T>
		{
			var visitor = new GraphStateMachineVisitor<T>();
			machine.Inspect(visitor);

			return CreateGraph(visitor.Vertices, visitor.Edges);
		}

		private void NodeStyler(object sender, GleeVertexEventArgs<Vertex> args)
		{
			if (args.Vertex.VertexType == typeof(Event))
			{
				args.Node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Red;
				args.Node.Attr.Label = args.Vertex.Title;
				args.Node.Attr.Shape = Shape.Ellipse;
			}
			else
			{
				
				args.Node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Blue;
				args.Node.Attr.Label = args.Vertex.Title;
				args.Node.Attr.Shape = Shape.Box;
			}

			args.Node.Attr.Fontcolor = Microsoft.Glee.Drawing.Color.White;
			args.Node.Attr.Fontsize = 8;
			args.Node.Attr.FontName = "Arial";
			args.Node.Attr.Padding = 1.2;
		}

		private static void EdgeStyler(object sender, GleeEdgeEventArgs<Vertex, Edge<Vertex>> e)
		{
			e.GEdge.EdgeAttr.FontName = "Tahoma";
			e.GEdge.EdgeAttr.Fontsize = 6;
		}
	}
}
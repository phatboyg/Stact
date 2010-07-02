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
namespace Magnum.Visualizers.Channels
{
	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Imaging;
	using Extensions;
	using Graphing;
	using Magnum.Channels;
	using Magnum.Channels.Visitors;
	using Microsoft.Glee.Drawing;
	using Microsoft.Glee.GraphViewerGdi;
	using QuickGraph;
	using QuickGraph.Glee;


	public class ChannelGraphGenerator
	{
		public Graph CreateGraph(ChannelGraphData data)
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

		public void SaveGraphToFile(Channel channel, int width, int height, string filename)
		{
			SaveGraphToFile(channel.GetGraphData(), width, height, filename);
		}

		public void SaveGraphToFile(ChannelGraphData data, int width, int height, string filename)
		{
			Graph gleeGraph = CreateGraph(data);

			var renderer = new GraphRenderer(gleeGraph);
			renderer.CalculateLayout();

			var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			renderer.Render(bitmap);

			Trace.WriteLine("Saving graph to " + filename);

			bitmap.Save(filename, ImageFormat.Png);
		}

		void NodeStyler(object sender, GleeVertexEventArgs<Vertex> args)
		{
			args.Node.Attr.Shape = Shape.Box;
			args.Node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Black;
			args.Node.Attr.Fontcolor = Microsoft.Glee.Drawing.Color.White;
			args.Node.Attr.Fontsize = 8;
			args.Node.Attr.FontName = "Tahoma";
			args.Node.Attr.Label = args.Vertex.Title;
			args.Node.Attr.Padding = 1.2;

			ApplyVertexStyle(args.Node.Attr, args.Vertex.VertexType);
		}

		static void ApplyVertexStyle(NodeAttr attr, Type type)
		{
			if (type.IsGenericType)
			{
				Type openType = type.GetGenericTypeDefinition();
				if (openType == typeof(TypedChannelAdapter<>))
				{
					attr.Shape = Shape.Diamond;
					attr.Fillcolor = Microsoft.Glee.Drawing.Color.Blue;
				}
				else if (openType == typeof(BroadcastChannel<>))
				{
					attr.Shape = Shape.Octagon;
					attr.Fillcolor = Microsoft.Glee.Drawing.Color.Yellow;
					attr.Fontcolor = Microsoft.Glee.Drawing.Color.Black;
				}
				else if (openType == typeof(IntervalChannel<>) || openType == typeof(DistinctChannel<,>)
				         || openType == typeof(ConvertChannel<,>) || openType == typeof(LastChannel<>))
				{
					attr.Shape = Shape.Ellipse;
					attr.Fillcolor = Microsoft.Glee.Drawing.Color.PowderBlue;
					attr.Fontcolor = Microsoft.Glee.Drawing.Color.Black;
				}
				else if (openType == typeof(FilterChannel<>) )
				{
					attr.Shape = Shape.Ellipse;
					attr.Fillcolor = Microsoft.Glee.Drawing.Color.YellowGreen;
					attr.Fontcolor = Microsoft.Glee.Drawing.Color.Black;
				}
				else if (openType == typeof(ConsumerChannel<>) || openType == typeof(InstanceChannel<>))
				{
					attr.Shape = Shape.Ellipse;
					attr.Fillcolor = Microsoft.Glee.Drawing.Color.Green;
				}
				else if (openType == typeof(InstanceChannelProvider<,>))
				{
					attr.Shape = Shape.Diamond;
					attr.Fillcolor = Microsoft.Glee.Drawing.Color.Purple;
				}
			}
			else
			{
				if (type == typeof(ChannelAdapter))
					attr.Fillcolor = Microsoft.Glee.Drawing.Color.Orange;
				else if (type == typeof(BroadcastChannel))
				{
					attr.Shape = Shape.Octagon;
					attr.Fillcolor = Microsoft.Glee.Drawing.Color.Yellow;
					attr.Fontcolor = Microsoft.Glee.Drawing.Color.Black;
				}
			}
		}

		static void EdgeStyler(object sender, GleeEdgeEventArgs<Vertex, Edge<Vertex>> e)
		{
			e.GEdge.EdgeAttr.Label = e.Edge.Source.TargetType.ToShortTypeName();
			e.GEdge.EdgeAttr.FontName = "Tahoma";
			e.GEdge.EdgeAttr.Fontsize = 6;
		}
	}
}
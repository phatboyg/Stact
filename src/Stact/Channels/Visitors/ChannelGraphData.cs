namespace Stact.Visitors
{
	using System;
	using System.Collections.Generic;
	using Magnum.Graphing;

	[Serializable]
	public class ChannelGraphData :
		GraphData
	{
		public ChannelGraphData(IEnumerable<Vertex> vertices, IEnumerable<Edge> edges)
			: base(vertices, edges)
		{
		}
	}
}
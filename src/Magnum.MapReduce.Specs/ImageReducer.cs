namespace Magnum.MapReduce.Specs
{
	using System;
	using System.Collections.Generic;
	using Mappers;

	internal class ImageReducer : IReducer<long, WebServerLogEntry, long, WebServerLogEntry>
	{
		public void Reduce(long key, IEnumerable<WebServerLogEntry> values, ICollector<long, WebServerLogEntry> collector)
		{
			foreach (WebServerLogEntry entry in values)
			{
				if(entry.UriStem.LastIndexOf(".jpg", StringComparison.InvariantCultureIgnoreCase) > 0)
				{
					collector.Collect(key, entry);
				}
			}
		}
	}
}
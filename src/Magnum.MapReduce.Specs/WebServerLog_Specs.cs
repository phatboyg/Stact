namespace Magnum.MapReduce.Specs
{
	using System;
	using System.Collections.Generic;
	using Common;
	using Mappers;
	using MbUnit.Framework;
	using Readers;
	using Types;

	[TestFixture]
	public class WebServerLog_Specs
	{
		private readonly string _filename = "ex" + DateTime.Now.ToString("yyMMdd") + ".log";
		private const string _baseUrl = "http://192.168.105.125/Web4Logs/";

		[Test]
		public void First_test()
		{
			IContentCollector contentCollector = new HttpContentCollector(_baseUrl + _filename);
			IContentReader contentReader = new BlockContentReader(contentCollector);
			LineRecordReader recordReader = new LineRecordReader(contentReader);

			WebServerLogMapper mapper = new WebServerLogMapper();

			var collector = new OutputCollector<long,WebServerLogEntry>();

			var runner = new MapRunner<long, string, long, WebServerLogEntry>(recordReader, mapper, collector);

			runner.Run();

			var outputReader = new DictionaryListRecordReader<long, WebServerLogEntry>(collector.Values);
			
			var reducer = new ImageReducer();

			var nextCollector = new OutputCollector<long, WebServerLogEntry>();

			var reduceRunner = new ReduceRunner<long, WebServerLogEntry, long, WebServerLogEntry>(outputReader, reducer, nextCollector);

			reduceRunner.Run();
			
		}
	}

	public class DictionaryListRecordReader<K,V> :
		IRecordReader<K,IEnumerable<V>> 
	{
		private readonly IDictionary<K, List<V>> _values;
		private IEnumerator<KeyValuePair<K, List<V>>> _enumerator;
		private long _index;
		private long _count;

		public DictionaryListRecordReader(IDictionary<K, List<V>> values)
		{
			_values = values;

			_index = 0;
			_count = _values.Count;

			_enumerator = _values.GetEnumerator();
		}

		public void Dispose()
		{
			_enumerator.Dispose();
			
		}

		public long Position
		{
			get { return _index; }
		}

		public float Progress
		{
			get { return (float)_count /_index;}
		}

		public void Close()
		{

		}

		public KeyValuePair<K, IEnumerable<V>> Next()
		{
			if (_enumerator.MoveNext())
			{
				_index++;

				return new KeyValuePair<K, IEnumerable<V>>(_enumerator.Current.Key, _enumerator.Current.Value);
			}

			return default(KeyValuePair<K, IEnumerable<V>>);
		}
	}
}
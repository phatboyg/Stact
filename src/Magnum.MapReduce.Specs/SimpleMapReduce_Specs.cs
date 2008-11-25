namespace Magnum.MapReduce.Specs
{
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;

	[TestFixture]
	public class When_mapping_an_array_to_another_array
	{
		private List<Source> _sourceList;

		[SetUp]
		public void Setup()
		{
			_sourceList = new List<Source>
				{
					new Source {Id = 1, Value = "A"},
					new Source {Id = 2, Value = "B"},
					new Source {Id = 1, Value = "A"},
					new Source {Id = 2, Value = "B"},
				};
		}

		[Test]
		public void We_should_be_able_to_make_sweet_love_to_it()
		{
			SourceMapper mapper = new SourceMapper();

			SourceReducer reducer = new SourceReducer();


		}
	}

	internal class SourceMapper : IMapper<long, Source, long, string>
	{
		public void Map(long key, Source value, ICollector<long, string> collector)
		{
			collector.Collect(value.Id, value.Value);
		}
	}

	internal class SourceReducer : IReducer<long, string, long, long>
	{
		public long Count { get; protected set; }

		public void Reduce(long key, IEnumerable<string> values, ICollector<long, long> collector)
		{
			Count++;

			long count = values.Count();

			collector.Collect(key, count);
		}
	}

	internal class Destination
	{
		public int Id { get; set; }
		public long Count { get; set; }
	}

	internal class Source
	{
		public int Id { get; set; }

		public string Value { get; set; }
	}
}
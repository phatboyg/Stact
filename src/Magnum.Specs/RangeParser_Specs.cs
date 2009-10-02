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
namespace Magnum.Specs
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Linq.Expressions;
	using NUnit.Framework;
	using Parsers;

	[TestFixture]
	public class Specifying_a_range_for_a_search_string :
		When_using_the_range_parser
	{
		protected override void Given()
		{
			base.Given();

			Range = "A-Z";
		}

		[Test, Explicit]
		public void Output_the_parse_results()
		{
			Elements.Each(x => Trace.WriteLine(x.ToString()));
		}

		[Test]
		public void Should_return_the_individual_ranges()
		{
			var expected = new RangeElement("A", "Z");

			Elements.Contains(expected).ShouldBeTrue();
		}
	}

	[TestFixture]
	public class Specifying_a_greater_than_range_for_a_search_string :
		When_using_the_range_parser
	{
		protected override void Given()
		{
			base.Given();

			Range = "K-";
		}

		[Test]
		public void Should_return_a_greater_than_range()
		{
			var expected = new GreaterThanElement("K");

			Elements.Contains(expected).ShouldBeTrue();
		}
	}

	[TestFixture]
	public class Specifying_a_less_than_range_for_a_search_string :
		When_using_the_range_parser
	{
		protected override void Given()
		{
			base.Given();

			Range = "-Z";
		}

		[Test]
		public void Should_return_a_less_than_range()
		{
			var expected = new LessThanElement("Z");

			Elements.Contains(expected).ShouldBeTrue();
		}
	}

	[TestFixture]
	public class Specifying_multiple_ranges_in_a_single_range_string :
		When_using_the_range_parser
	{
		protected override void Given()
		{
			base.Given();

			Range = "A-J;P-S";
		}

		[Test]
		public void Should_return_the_first_range()
		{
			var expected = new RangeElement("A", "J");

			Elements.Contains(expected).ShouldBeTrue();
		}

		[Test]
		public void Should_return_the_second_range()
		{
			var expected = new RangeElement("P", "S");

			Elements.Contains(expected).ShouldBeTrue();
		}
	}

	[TestFixture]
	public class Specifying_only_a_search_pattern :
		When_using_the_range_parser
	{
		protected override void Given()
		{
			base.Given();

			Range = "ABC";
		}

		[Test]
		public void Should_return_the_pattern()
		{
			var expected = new StartsWithElement("ABC");

			Elements.Contains(expected).ShouldBeTrue();
		}
	}

	[TestFixture]
	public class Specifying_an_out_of_order_range :
		When_using_the_range_parser_with_the_optimizer
	{
		protected override void Given()
		{
			base.Given();

			Range = "F-A";
			ExpectedRange = "A-F";
		}

		[Test]
		public void Should_adjust_the_range_to_the_proper_order()
		{
			var expected = new RangeElement("A", "F");

			Elements.Contains(expected).ShouldBeTrue();
		}
	}

	[TestFixture]
	public class Specifying_multiple_starting_patterns :
		When_using_the_range_parser
	{
		protected override void Given()
		{
			base.Given();

			Range = "ABC;DEF;GHI";
		}

		[Test]
		public void Should_return_the_first_pattern()
		{
			var expected = new StartsWithElement("ABC");

			Elements.Contains(expected).ShouldBeTrue();
		}

		[Test]
		public void Should_return_the_second_pattern()
		{
			var expected = new StartsWithElement("DEF");

			Elements.Contains(expected).ShouldBeTrue();
		}

		[Test]
		public void Should_return_the_third_pattern()
		{
			var expected = new StartsWithElement("GHI");

			Elements.Contains(expected).ShouldBeTrue();
		}
	}

	[TestFixture]
	public class Specify_a_bunch_of_different_patterns_and_ranges :
		When_using_the_range_parser
	{
		protected override void Given()
		{
			base.Given();

			Range = "ABC;A-G;DEF;H-K;GHI;M-N";
		}

		[Test]
		public void Should_return_the_first_pattern()
		{
			var expected = new StartsWithElement("ABC");

			Elements.Contains(expected).ShouldBeTrue();
		}

		[Test]
		public void Should_return_the_second_pattern()
		{
			var expected = new StartsWithElement("DEF");

			Elements.Contains(expected).ShouldBeTrue();
		}

		[Test]
		public void Should_return_the_third_pattern()
		{
			var expected = new StartsWithElement("GHI");

			Elements.Contains(expected).ShouldBeTrue();
		}

		[Test]
		public void Should_return_the_first_range()
		{
			var expected = new RangeElement("A", "G");

			Elements.Contains(expected).ShouldBeTrue();
		}

		[Test]
		public void Should_return_the_second_range()
		{
			var expected = new RangeElement("H", "K");

			Elements.Contains(expected).ShouldBeTrue();
		}

		[Test]
		public void Should_return_the_third_range()
		{
			var expected = new RangeElement("M", "N");

			Elements.Contains(expected).ShouldBeTrue();
		}
	}

	[TestFixture]
	public class Specifying_overlapping_ranges_where_the_first_is_wider :
		When_using_the_range_parser_with_the_optimizer
	{
		protected override void Given()
		{
			base.Given();

			Range = "A-K;C-G";
			ExpectedRange = "A-K";
		}

		[Test]
		public void Should_optimize_out_the_redundant_ranges()
		{
			var expected = new RangeElement("A", "K");

			Elements.Contains(expected).ShouldBeTrue();
		}

		[Test]
		public void Should_return_the_second_range()
		{
			var expected = new RangeElement("C", "G");

			Elements.Contains(expected).ShouldBeFalse();
		}
	}

	[TestFixture]
	public class Specifying_a_range_included_in_a_greater_than :
		When_using_the_range_parser_with_the_optimizer
	{
		protected override void Given()
		{
			base.Given();

			Range = "B-;G-M";
			ExpectedRange = "B-";
		}

		[Test]
		public void Should_not_include_the_range()
		{
			var expected = new RangeElement("G", "M");

			Elements.Contains(expected).ShouldBeFalse();
		}

		[Test]
		public void Should_include_the_greater_than()
		{
			var expected = new GreaterThanElement("B");

			Elements.Contains(expected).ShouldBeTrue();
		}
	}

	[TestFixture]
	public class Specifying_a_wider_range_after_a_narrow_range :
		When_using_the_range_parser_with_the_optimizer
	{
		protected override void Given()
		{
			base.Given();

			Range = "G-M;B-P";
			ExpectedRange = "B-P";
		}

		[Test]
		public void Should_include_the_wide_range()
		{
			var expected = new RangeElement("B", "P");

			Elements.Contains(expected).ShouldBeTrue();
		}

		[Test]
		public void Should_not_include_the_narrow_range()
		{
			var expected = new RangeElement("G", "M");

			Elements.Contains(expected).ShouldBeFalse();
		}
	}

	[TestFixture]
	public class Specifying_an_overlapping_range :
		When_using_the_range_parser_with_the_optimizer
	{
		protected override void Given()
		{
			base.Given();

			Range = "A-D;B-G;P-S;O-Q";
			ExpectedRange = "A-G;O-S";
		}

		[Test]
		public void Should_include_the_first_range()
		{
			var expected = new RangeElement("A", "G");

			Elements.Contains(expected).ShouldBeTrue();
		}

		[Test]
		public void Should_include_the_second_range()
		{
			var expected = new RangeElement("O", "S");

			Elements.Contains(expected).ShouldBeTrue();
		}

		[Test]
		public void Should_include_only_one_range()
		{
			Elements.Length.ShouldEqual(2);
		}
	}

	[TestFixture]
	public class Specifying_a_range_with_the_same_begin_and_end :
		When_using_the_range_parser_with_the_optimizer
	{
		protected override void Given()
		{
			base.Given();

			Range = "BA-BA";
			ExpectedRange = "BA";
		}

		[Test]
		public void Should_not_include_the_range()
		{
			var expected = new RangeElement("BA", "BA");

			Elements.Contains(expected).ShouldBeFalse();
		}

		[Test]
		public void Should_include_the_starts_with()
		{
			var expected = new StartsWithElement("BA");

			Elements.Contains(expected).ShouldBeTrue();
		}
	}

	[TestFixture]
	public class Specifying_a_numeric_range :
		When_using_the_range_parser_with_the_optimizer
	{
		protected override void Given()
		{
			base.Given();

			Range = "100-110";
			ExpectedRange = "100-110";
		}

		[Test]
		public void Should_include_the_appropriate_range()
		{
			var expected = new RangeElement("100", "110");

			Elements.Contains(expected).ShouldBeTrue();
		}
	}

	[TestFixture]
	public class Specifying_overlapping_ranges_where_the_second_is_wider :
		When_using_the_range_parser_with_the_optimizer
	{
		protected override void Given()
		{
			base.Given();

			Range = "C-G;A-K;B;C;K";
			ExpectedRange = "A-K";
		}

		[Test]
		public void Should_optimize_out_the_redundant_ranges()
		{
			var expected = new RangeElement("A", "K");

			Elements.Contains(expected).ShouldBeTrue();
		}

		[Test]
		public void Should_not_contain_a_second_group()
		{
			var expected = new RangeElement("C", "G");

			Elements.Contains(expected).ShouldBeFalse();
		}

		[Test]
		public void Should_not_contain_starts_with_entries()
		{
			Elements.Contains(new StartsWithElement("B")).ShouldBeFalse();
			Elements.Contains(new StartsWithElement("C")).ShouldBeFalse();
			Elements.Contains(new StartsWithElement("K")).ShouldBeFalse();
		}
	}

	[TestFixture]
	public class When_using_the_range_parser_with_the_optimizer :
		BehaviorTest
	{
		protected IRangeParser Parser { get; set; }
		protected IRangeElement[] Elements { get; set; }
		protected string Range { get; set; }
		protected string ExpectedRange { get; set; }

		protected override void Given()
		{
			Parser = new RangeParser();
			Range = "";
			ExpectedRange = "";
		}

		protected override void When()
		{
			Elements = Parser.Parse(Range).Optimize().CombineOverlappingRanges().ToArray();
		}

		[Test]
		public void Should_return_the_proper_display_format()
		{
			Elements.ToRangeString().ShouldEqual(ExpectedRange);
		}
	}

	[TestFixture]
	public class When_using_the_range_parser :
		BehaviorTest
	{
		protected IRangeParser Parser { get; set; }
		protected IRangeElement[] Elements { get; set; }
		protected string Range { get; set; }

		protected override void Given()
		{
			Parser = new RangeParser();
			Range = "";
		}

		protected override void When()
		{
			Elements = Parser.Parse(Range).ToArray();
		}

		[Test]
		public void Should_return_the_proper_display_format()
		{
			Elements.ToRangeString().ShouldEqual(Range);
		}
	}

	[TestFixture]
	public class Projecting_a_less_than_range_to_an_expression :
		When_using_the_range_parser_with_the_expression_projector
	{
		protected override void Given()
		{
			base.Given();

			Range = "-K";
		}

		[Test]
		public void Should_return_the_appropriate_rows()
		{
			Results.Count.ShouldEqual(4);
		}
	}

	[TestFixture]
	public class Projecting_a_less_than_range_with_multiple_characters_to_an_expression :
		When_using_the_range_parser_with_the_expression_projector
	{
		protected override void Given()
		{
			base.Given();

			Range = "-Omd";
		}

		[Test]
		public void Should_return_the_appropriate_rows()
		{
			Results.Count.ShouldEqual(4);
		}
	}

	[TestFixture]
	public class Projecting_a_range_from_a_to_an_expression :
		When_using_the_range_parser_with_the_expression_projector
	{
		protected override void Given()
		{
			base.Given();

			Range = "De-";
		}

		[Test]
		public void Should_return_the_appropriate_rows()
		{
			Results.Count.ShouldEqual(4);
		}
	}

	[TestFixture]
	public class Projecting_a_range_through_z_to_an_expression :
		When_using_the_range_parser_with_the_expression_projector
	{
		protected override void Given()
		{
			base.Given();

			Range = "-Z";
		}

		[Test]
		public void Should_return_the_appropriate_rows()
		{
			Results.Count.ShouldEqual(6);
		}
	}

	[TestFixture]
	public class Projecting_a_range_to_an_expression :
		When_using_the_range_parser_with_the_expression_projector
	{
		protected override void Given()
		{
			base.Given();

			Range = "G-O";
		}

		[Test]
		public void Should_return_the_appropriate_rows()
		{
			Results.Count.ShouldEqual(2);
		}
	}

	[TestFixture]
	public class Projecting_a_starts_with_to_an_expression :
		When_using_the_range_parser_with_the_expression_projector
	{
		protected override void Given()
		{
			base.Given();

			Range = "G";
		}

		[Test]
		public void Should_return_the_appropriate_rows()
		{
			Results.Count.ShouldEqual(1);
		}
	}

	[TestFixture]
	public class Projecting_two_ranges_should_include_both_sets_of_results :
		When_using_the_range_parser_with_the_expression_projector
	{
		protected override void Given()
		{
			base.Given();

			Range = "A-B;O-Z";
		}

		[Test]
		public void Should_return_the_appropriate_rows()
		{
			Results.Count.ShouldEqual(4);
		}
	}

	[TestFixture]
	public class When_using_the_range_parser_with_the_expression_projector :
		BehaviorTest
	{
		protected IRangeParser Parser { get; set; }
		protected IRangeElement[] Elements { get; set; }
		protected string Range { get; set; }
		protected IList<Company> Companies { get; set; }
		protected IList<Company> Results { get; set; }

		protected override void Given()
		{
			Companies = new List<Company>
				{
					new Company { Name = "Alpha"},
					new Company { Name = "Beta"},
					new Company { Name = "Gamma"},
					new Company { Name = "Delta"},
					new Company { Name = "Omega"},
					new Company { Name = "Zeta"},
				};

			Parser = new RangeParser();
			Range = "";
		}

		protected override void When()
		{
			Elements = Parser.Parse(Range).Optimize().CombineOverlappingRanges().ToArray();

			Results = Companies.WhereInRange(x => x.Name, Elements).ToList();
		}

		protected class Company
		{
			public string Name { get; set; }
		}
	}

	[TestFixture]
	public class Specifying_a_number_range_to_query :
		When_using_the_range_parser_with_the_numeric_expression_projector
	{
		protected override void Given()
		{
			base.Given();

			Range = "100-110";
		}

		[Test]
		public void Should_include_the_appropriate_range()
		{
			var expected = new RangeElement("100", "110");

			Elements.Contains(expected).ShouldBeTrue();
		}

		[Test]
		public void Should_include_the_matching_values()
		{
			Results.Count.ShouldEqual(4);
		}
	}

	[TestFixture]
	public class When_using_the_range_parser_with_the_numeric_expression_projector :
		BehaviorTest
	{
		protected IRangeParser Parser { get; set; }
		protected IRangeElement[] Elements { get; set; }
		protected string Range { get; set; }
		protected IList<Code> Codes { get; set; }
		protected IList<Code> Results { get; set; }

		protected override void Given()
		{
			Codes = new List<Code>
				{
					new Code { Value = 100},
					new Code { Value = 102},
					new Code { Value = 104},
					new Code { Value = 110},
					new Code { Value = 120},
					new Code { Value = 130},
				};

			Parser = new RangeParser();
			Range = "";
		}

		protected override void When()
		{
			Elements = Parser.Parse(Range).Optimize().CombineOverlappingRanges().ToArray();

			Results = Codes.WhereInRange(x => x.Value, Elements).ToList();
		}

		protected class Code
		{
			public int Value { get; set; }
		}
	}
}
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
namespace Stact.Specs.Linq
{
	using System.Linq;
	using NUnit.Framework;
	using Stact.Linq;

	[TestFixture]
	public class Indexing_an_enumeration
	{
		[Test]
		public void Should_return_an_indexed_enumeration_of_a_new_type()
		{
			var strings = new[] {"A", "B", "C"};

			var result = strings.Index((x,i) => new { Index = i, Value = x});

			Assert.IsNotNull(result);

			var resultArray = result.ToArray();

			Assert.AreEqual(strings.Length, resultArray.Length);

			Assert.AreEqual(resultArray[0].Index, 0);
			Assert.AreEqual(resultArray[1].Index, 1);
		}

		[Test]
		public void Should_allow_starting_at_another_index()
		{
			var strings = new[] {"A", "B", "C"};

			var result = strings.Index(47, (x,i) => new { Index = i, Value = x});

			Assert.IsNotNull(result);

			var resultArray = result.ToArray();

			Assert.AreEqual(strings.Length, resultArray.Length);

			Assert.AreEqual(resultArray[0].Index, 47);
			Assert.AreEqual(resultArray[1].Index, 48);
		}
	}
}
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
namespace Magnum.Common.Specs
{
	using NUnit.Framework;

	[TestFixture]
	public class When_mapping_an_object_to_an_object
	{
		[Test]
		public void It_should_just_work()
		{
			Mapper<SourceObject, TargetObject> map = new Mapper<SourceObject, TargetObject>();

			map.From(x => x.Id).To((y, x) => y.CustomerId = x);
			map.From(x => x.Name).To((y, x) => y.DisplayName = x);
			map.From(x => x.Amount).To((y, x) => y.OrderAmount = x);

			SourceObject source = new SourceObject
				{
					Id = 27,
					Name = "Chris",
					Amount = 234.75m,
				};

			TargetObject target = map.Transform(source);

			Assert.AreEqual(source.Id, target.CustomerId);
			Assert.AreEqual(source.Name, target.DisplayName);
			Assert.AreEqual(source.Amount, target.OrderAmount);
		}
	}

	internal class TargetObject
	{
		public int CustomerId { get; set; }

		public string DisplayName { get; set; }

		public decimal OrderAmount { get; set; }
	}

	internal class SourceObject
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public decimal Amount { get; set; }
	}
}
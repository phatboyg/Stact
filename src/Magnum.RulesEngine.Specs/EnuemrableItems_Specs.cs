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
namespace Magnum.RulesEngine.Specs
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Model;
	using NUnit.Framework;

	[TestFixture]
	public class When_selecting_against_a_rich_object_model_with_collections
	{
		[Test]
		public void FirstTestName()
		{
			Order order = new Order
				{
					OrderLines = new List<OrderLine>
						{
							new OrderLine {ProductCode = 12345},
							new OrderLine {ProductCode = 67890},
						},
				};


			List<Order> orders = new List<Order>
				{
					order
				};

			IQueryable<IGrouping<int, OrderLine>> qx = from o in orders.AsQueryable()
			         from ol in o.OrderLines
			         group ol by ol.ProductCode
			         into lines
			         	let l1 = lines.Count(x => x.ProductCode == 12345)
			         	let l2 = lines.Count(x => x.ProductCode == 67890)
			         	where l1 > 0 && l2 > 0
			         	select lines;

			Trace.WriteLine(qx.Expression);

			var query = orders
				.Select(o => new
					{
						o,
						l1c = o.OrderLines
				             	.Where(x => x.ProductCode == 12345)
				             	.Count(),
						l2c = o.OrderLines.Where(x => x.ProductCode == 12345).Count()
					})
				.Where(@t => @t.l1c > 0)
				.Where(@t => @t.l2c > 0)
				.Select(@t => @t.o);
		}
	}
}
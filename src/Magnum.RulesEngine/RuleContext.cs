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
namespace Magnum.RulesEngine
{
	using System;
	using System.Collections.Generic;
	using ExecutionModel;

	/*
	 * I may look to try and provide some sort of monadic selectmany style of context
	 * similar to how jQuery passing matching elements to the next item in the chain
	 * 
	 * This would be interesting in that it would allow conditions to be built in a similar way
	 * 
	 * offer up a type of <T, IEnumerable<T> as a tuple or something for dispatching
	 * 
	 * This would be a mulitiple Input node type of thing:
	 * 
	 * from order in memory<order>
	 * from orderline in order.orderlines
	 * where orderline.quantity > 10
	 * from otherorderline in order.orderlines
	 * where otherorderline.quantity > 10
	 * 
	 * 
	 * from claim in memory<claim>
	 * from serviceline in claim.servicelines where serviceline.revenuecode == '300'
	 * from servicelin2 in claim.servicelines where servicelin2.revenuecode == '280'
	 * where serviceline.count() > 0 and servicelin2.count() > 0
	 * select claim.claimid;
	 * 
	 * this should match the orders where two orderlines have quantities > 10
	 * 
	 * where <order>
	 *	where 
	 * 
	 * 
	 * 
	 */


	public interface RuleContext
	{
		Type ItemType { get; }

		void EnqueueAgendaAction(Action action);
		void EnqueueAgendaAction(int priority, Action action);
	}

	public interface RuleContext<T> :
		RuleContext
	{
		WorkingMemoryElement<T> Element { get; }

		void AddElementToAlphaMemory(int key, WorkingMemoryElement<T> element, IEnumerable<Node> successors);
	}
}
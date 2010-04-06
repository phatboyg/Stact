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
namespace Magnum.RulesEngine.ExecutionModel
{
	using System;
	using System.Collections.Generic;
	using Extensions;

	public class SessionRuleContext<T> :
		RuleContext<T>
	{
		private readonly Dictionary<int, BetaMemory<T>> _betaMemory;
		private readonly StatefulSessionImpl _session;

		public SessionRuleContext(StatefulSessionImpl session, SessionElement<T> item)
		{
			_session = session;
			_betaMemory = new Dictionary<int, BetaMemory<T>>();

			Element = item;
		}

		public SessionElement<T> Element { get; private set; }

		public T Object
		{
			get { return Element.Object; }
		}

		public BetaMemory<T> GetBetaMemory(int key, Func<BetaMemory<T>> onMissing)
		{
			return _betaMemory.Retrieve(key, onMissing);
		}

		public Type ElementType
		{
			get { return typeof (T); }
		}

		public void EnqueueAgendaAction(Action action)
		{
			_session.Agenda.Add(action);
		}

		public void EnqueueAgendaAction(int priority, Action action)
		{
			_session.Agenda.Add(priority, action);
		}
	}
}
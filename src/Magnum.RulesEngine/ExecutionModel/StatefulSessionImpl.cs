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

	public class StatefulSessionImpl :
		StatefulSession
	{
		private readonly WorkingMemoryElementList _elements;
		private readonly RulesEngine _engine;
		private bool _disposed;

		public StatefulSessionImpl(RulesEngine engine)
		{
			_engine = engine;
			_elements = new WorkingMemoryElementList();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Assert<T>(T obj)
		{
			WorkingMemoryElement<T> element = new SessionWorkingMemoryElement<T>(this, obj);
			_elements.Add(element);

			var context = new SessionRuleContext<T>(this, element);

			_engine.Assert(context);

			context.DumpMemory();
		}

		private void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
			}

			_disposed = true;
		}

		~StatefulSessionImpl()
		{
			Dispose(false);
		}
	}

	public static class SessionExtensions
	{
		public static StatefulSession CreateSession(this RulesEngine engine)
		{
			return new StatefulSessionImpl(engine);
		}
	}
}
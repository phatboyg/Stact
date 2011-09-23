// Copyright 2010 Chris Patterson
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
namespace Stact.Internal
{
	using System;
	using System.Collections.Generic;


	public class TryCatchOperationExecutor :
		OperationExecutor
	{
		readonly Action<Exception> _callback;
		bool _stopping;

		public TryCatchOperationExecutor(Action<Exception> callback)
		{
			_callback = callback;
		}


		public void Execute(Action operation)
		{
			try
			{
				if (_stopping)
					return;

				operation();
			}
			catch (Exception ex)
			{
				_callback(ex);
			}
		}

		public void Execute(IList<Action> operations)
		{
			try
			{
				for (int index = 0; index < operations.Count;)
				{
					if (_stopping)
						break;

					// this is nested to avoid the cost of a try/catch block per operation
					// but uses two blocks for a single operation -- tradeoffs I guess

					try
					{
						for (; index < operations.Count; index++)
						{
							if (_stopping)
								break;

							operations[index]();
						}
					}
					catch (Exception ex)
					{
						_callback(ex);
						index++;
					}
				}
			}
			catch (Exception ex)
			{
				_callback(ex);
			}
		}

		public void Stop()
		{
			_stopping = true;
		}
	}
}
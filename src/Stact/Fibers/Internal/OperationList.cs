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


	public class OperationList
	{
		IList<Action> _operations;

		public OperationList()
		{
			_operations = new List<Action>();
		}

		public void Add(Action operation)
		{
			_operations.Add(operation);
		}

		public void Run()
		{
			if (_operations.Count == 0)
				return;

			IList<Action> operations = _operations;
			_operations = new List<Action>();

			foreach (Action operation in operations)
				operation();
		}
	}
}
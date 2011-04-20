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
namespace Stact.Routing.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Generic;


	public class SuccessorList<TChannel> :
		IEnumerable<Activation<TChannel>>
	{
		readonly IList<Activation<TChannel>> _activations;

		public SuccessorList(params Activation<TChannel>[] activations)
		{
			_activations = new List<Activation<TChannel>>(activations);
		}

		public IEnumerator<Activation<TChannel>> GetEnumerator()
		{
			for (int i = 0; i < _activations.Count;)
			{
				if (!_activations[i].Enabled)
				{
					_activations.RemoveAt(i);
					continue;
				}

				yield return _activations[i];

				i++;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(Activation<TChannel> activation)
		{
			_activations.Insert(0, activation);
		}

		public void All(Action<Activation<TChannel>> callback)
		{
			foreach (var activation in this)
				callback(activation);
		}

		public void Remove(Activation<TChannel> activation)
		{
			_activations.Remove(activation);
		}
	}
}
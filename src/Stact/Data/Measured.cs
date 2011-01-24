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
namespace Stact.Data
{
	using System;
	using Internal;


	public class Measured<T, M>
	{
		public readonly M Identity;
		readonly Func<T, M> _measure;
		readonly Monoid<M> _monoid;
		Measured<Node<T, M>, M> _measureNode;

		public Measured(Monoid<M> monoid, Func<T, M> measure)
		{
			_monoid = monoid;
			_measure = measure;

			Identity = monoid.Identity;
		}

		public Measured<Node<T, M>, M> Node
		{
			get { return _measureNode ?? (_measureNode = new Measured<Node<T, M>, M>(_monoid, node => node.Size)); }
		}

		public Element<T,M> Measure(T item)
		{
			return new Element<T, M>(_measure(item), item);
		}

		public M Append(M left, M right)
		{
			return _monoid.Append(left, right);
		}
	}
}
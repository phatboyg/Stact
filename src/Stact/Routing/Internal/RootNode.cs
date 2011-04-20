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
	using System.Collections.Generic;
	using Magnum.Collections;
	using Magnum.Extensions;
	using Stact.Internal;


	public class RootNode :
		Activation
	{
		readonly Cache<Type, Activation> _types;

		public RootNode()
		{
			_types = new Cache<Type, Activation>();
		}

		public IEnumerable<Activation> Activations
		{
			get { return _types; }
		}

		public void Activate<T>(RoutingContext<T> context)
		{
			Activation activation = _types.Retrieve(typeof(T), _ => new AlphaNode<T>());

			activation.Activate(context);
		}

		[NotNull]
		public AlphaNode<T> GetAlphaNode<T>()
		{
			Activation activation = _types.Retrieve(typeof(T), _ => new AlphaNode<T>());
			if (activation.GetType() == typeof(AlphaNode<T>))
				return (AlphaNode<T>)activation;

			throw new InvalidOperationException(
				"The activation for {0} is not an Alpha node".FormatWith(typeof(T).ToShortTypeName()));
		}
	}
}
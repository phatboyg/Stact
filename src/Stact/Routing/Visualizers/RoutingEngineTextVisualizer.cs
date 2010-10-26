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
namespace Stact.Routing.Visualizers
{
	using System;
	using System.Diagnostics;
	using Internal;
	using Magnum.Extensions;


	public class RoutingEngineTextVisualizer :
		AbstractRoutingEngineVisitor<RoutingEngineTextVisualizer>
	{
		Action<string> _output = s => Trace.WriteLine(s);

		public Action<string> Output
		{
			set { _output = value ?? NoOutput; }
		}

		static void NoOutput(string s)
		{
		}

		protected override bool Visit(DynamicRoutingEngine engine)
		{
			_output(engine.GetType().ToShortTypeName());

			return base.Visit(engine);
		}

		protected override bool Visit(TypeRouter router)
		{
			_output(router.GetType().ToShortTypeName());

			return base.Visit(router);
		}

		protected override bool Visit<TChannel>(AlphaNode<TChannel> node)
		{
			_output(node.GetType().ToShortTypeName());

			return base.Visit(node);
		}

		protected override bool Visit<TChannel>(JoinNode<TChannel> node)
		{
			_output(node.GetType().ToShortTypeName());

			return base.Visit(node);
		}

		protected override bool Visit<T1, T2>(JoinNode<T1, T2> node)
		{
			_output(node.GetType().ToShortTypeName());

			return base.Visit(node);
		}

		protected override bool Visit<TChannel>(ConstantNode<TChannel> node)
		{
			_output(node.GetType().ToShortTypeName());

			return base.Visit(node);
		}

		protected override bool Visit<TChannel>(ConsumerNode<TChannel> node)
		{
			_output(node.GetType().ToShortTypeName());

			return base.Visit(node);
		}
	}
}
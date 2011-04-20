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
	using System.Text;
	using Internal;
	using Magnum.Extensions;


	public class StringRoutingEngineVisitor :
		AbstractRoutingEngineVisitor<StringRoutingEngineVisitor>
	{
		StringBuilder _sb = new StringBuilder();

		public StringRoutingEngineVisitor(RoutingEngine engine)
		{
			Visit(engine);
		}

		public override string ToString()
		{
			return _sb.ToString();
		}

		protected override bool Visit(DynamicRoutingEngine engine)
		{
			_sb.AppendLine(engine.GetType().ToShortTypeName());

			return base.Visit(engine);
		}

		protected override bool Visit(RootNode router)
		{
			_sb.AppendLine(router.GetType().ToShortTypeName());

			return base.Visit(router);
		}

		protected override bool Visit<T>(AlphaNode<T> node)
		{
			_sb.AppendLine(node.GetType().ToShortTypeName());

			return base.Visit(node);
		}

		protected override bool Visit<T>(JoinNode<T> node)
		{
			_sb.AppendLine(node.GetType().ToShortTypeName());

			return base.Visit(node);
		}

		protected override bool Visit<T1, T2>(JoinNode<T1, T2> node)
		{
			_sb.AppendLine(node.GetType().ToShortTypeName());

			return base.Visit(node);
		}

		protected override bool Visit<T>(BodyNode<T> node)
		{
			_sb.AppendLine(node.GetType().ToShortTypeName());

			return base.Visit(node);
		}

		protected override bool Visit<T1In, T2In, T1, T2>(BodyNode<T1In, T2In, T1, T2> node)
		{
			_sb.AppendLine(node.GetType().ToShortTypeName());

			return base.Visit(node);
		}

		protected override bool Visit<T>(ConstantNode<T> node)
		{
			_sb.AppendLine(node.GetType().ToShortTypeName());

			return base.Visit(node);
		}

		protected override bool Visit<T>(ConsumerNode<T> node)
		{
			_sb.AppendLine(node.GetType().ToShortTypeName());

			return base.Visit(node);
		}

		protected override bool Visit<T>(SelectiveConsumerNode<T> node)
		{
			_sb.AppendLine(node.GetType().ToShortTypeName());

			return base.Visit(node);
		}
	}
}
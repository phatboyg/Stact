namespace Magnum.RulesEngine.Specs.ExecutionModelTest
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using ExecutionModel;
	using Model;
	using NUnit.Framework;

	[TestFixture]
	public class JoinTree_Specs
	{
		private StringNodeVisitor _visitor;
		private MemoryJunction<Customer> _junction;

		[SetUp]
		public void Setup()
		{
			_visitor = new StringNodeVisitor();
		}

		[TearDown]
		public void Teardown()
		{
			_junction.Visit(_visitor);
			Trace.WriteLine(_visitor.Result);
		}

		private IEnumerable<AlphaNode<T>> Get<T>(int count)
		{
			for (int i = 0; i < count; i++)
			{
				yield return new AlphaNode<T>();
			}
		}

		[Test]
		public void A_single_alpha_node()
		{
			_junction = Get<Customer>(1).GetFinalJunction();
		}

		[Test]
		public void Two_alpha_nodes()
		{
			_junction = Get<Customer>(2).GetFinalJunction();
		}
	}

	public static class xxx
	{
		public static MemoryJunction<T> GetFinalJunction<T>(this IEnumerable<AlphaNode<T>> nodes)
		{
			MemoryJunction<T> junction;

			if (nodes.Count() == 1)
			{
				junction = new MemoryJunction<T>(new ConstantNode<T>());
				junction.AddSuccessor(nodes.Single());
			}
			else if (nodes.Count() == 2)
			{
				junction = new MemoryJunction<T>(nodes.First());
				nodes.Skip(1).First().AddSuccessor(junction);
			}
			else
			{

				junction = new MemoryJunction<T>(nodes.First());
				nodes.Skip(1).GetFinalJunction().AddSuccessor(junction);
			}

			return junction;
		}

		public static MemoryJunction<T> CreateJunction<T>(this IEnumerable<AlphaNode<T>> nodes, RightActivation<T> rightActivation)
		{
			var junction = new MemoryJunction<T>(rightActivation);

//			junction.AddSuccessor(nodes.CreateJunction());
			throw new NotImplementedException();
		}
	}
}

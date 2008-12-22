namespace Magnum.RulesEngine.Specs
{
	using MbUnit.Framework;
	using Model;

	[TestFixture]
	public class SimpleRule_Specs
	{
		[Test]
		public void A_condition_should_properly_evaluate_the_object()
		{
			Person p = new Person {FirstName = "Chris", LastName = "Patterson"};

			ICondition condition = new Condition<Person>(x => x.FirstName == "Chris");

			bool result = condition.Evaluate(p);

			Assert.IsTrue(result);
		}

		[Test]
		public void A_condition_that_does_not_match_the_object_type_should_return_false()
		{
			Address a = new Address { Street = "123 American Way", City = "Anytown", State = "OK", Country = "USA" };

			ICondition condition = new Condition<Person>(x => x.FirstName == "Chris");

			bool result = condition.Evaluate(a);

			Assert.IsFalse(result);
		}

		[Test]
		public void A_double_condition_should_match_if_matching()
		{
			Address a = new Address { Street = "123 American Way", City = "Anytown", State = "OK", Country = "USA" };
			Address b = new Address { Street = "456 American Way", City = "Anothertown", State = "OK", Country = "USA" };

			ICondition condition = new Condition<Address, Address>((x, y) => x.State == y.State);
			
		}
	}
}
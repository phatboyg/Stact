namespace Magnum.Core.Tests
{
	using System;
	using CheckHelpers;
	using NUnit.Framework;

	[TestFixture]
	public class To_verify_that_an_argument_is_not_null : Specification
	{
		[Test]
		public void A_full_string_should_be_handled_silently()
		{
			string name = "Chris";

			Check.That(name, Is.Not.Empty);
		}

		[Test]
		public void A_valid_argument_should_be_handled_silently()
		{
			string name = "Chris";

			Check.Argument("name", name, Is.Not.Null);
		}

		[Test]
		public void An_empty_string_check_should_not_throw_an_exception_for_a_valid_string()
		{
		}

		[Test, ExpectedException(typeof (CheckException))]
		public void An_empty_string_should_throw_an_exception()
		{
			string name = "";

			Check.That(name, Is.Not.Empty);
		}

		[Test, ExpectedException(typeof (CheckException))]
		public void An_exception_should_be_thrown()
		{
			string name = null;

			Check.That(name, Is.Not.Null);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void An_argument_exception_should_be_thrown()
		{
			string name = null;

			Check.Argument("name", name, Is.Not.Null);
		}
	}

	[TestFixture]
	public class When_an_input_parameter_contains_a_value : Specification
	{
		[Test]
		public void Then_no_exception_should_be_thrown()
		{
			string argument = "value";

			Check.That(argument, Is.Not.Empty);
		}
	}

	[TestFixture]
	public class When_an_empty_argument_is_expected : Specification
	{
		[Test, ExpectedException(typeof (CheckException))]
		public void Then_an_argument_with_a_value_should_throw_an_exception()
		{
			string argument = "value";

			Check.That(argument, Is.Empty);
		}
	}
}
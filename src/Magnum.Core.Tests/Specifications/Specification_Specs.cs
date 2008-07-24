namespace Magnum.Core.Tests.Specifications
{
	using Magnum.Core.Specification;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Validators;

	[TestFixture]
	public class Specification_Specs : Specification
	{
		[Test]
		public void Compare_a_number()
		{
			Member member = new Member();
			member.Attempts = 7;

			ISpecification<Member> specification = new Specification<Member>().Equal("Attempts", 5);

			Assert.That(specification.IsSatisfiedBy(member), Is.False);
		}

		[Test]
		public void Compare_a_number_goodly()
		{
			Member member = new Member();
			member.Attempts = 7;

			ISpecification<Member> specification = new Specification<Member>().Equal("Attempts", 7);

			Assert.That(specification.IsSatisfiedBy(member), Is.True);
		}

		[Test]
		public void Compare_two_different_values()
		{
			Member member = new Member();
			member.Username = "joey";
			member.Attempts = 7;

			ISpecification<Member> specification = new Specification<Member>()
				.Equal("Username", "joey")
				.And
				.Equal("Attempts", 9);

			Assert.That(specification.IsSatisfiedBy(member), Is.False);
		}

		[Test]
		public void Compare_two_values()
		{
			Member member = new Member();
			member.Username = "joey";
			member.Attempts = 7;

			ISpecification<Member> specification = new Specification<Member>()
				.Equal("Username", "joey")
				.And
				.Equal("Attempts", 7);

			Assert.That(specification.IsSatisfiedBy(member), Is.True);
		}

		[Test]
		public void Not_a_match()
		{
			Member member = new Member();
			member.Username = "jimbo";

			ISpecification<Member> specification = new Specification<Member>().Equal("Username", "joey");

			Assert.That(specification.IsSatisfiedBy(member), Is.False);
		}

		[Test]
		public void Specify_a_fluent_speficiation()
		{
			Member member = new Member();
			member.Username = "joey";

			ISpecification<Member> specification = new Specification<Member>().Equal("Username", "joey");

			Assert.That(specification.IsSatisfiedBy(member), Is.True);
		}
	}

	[TestFixture]
	public class When_specifying_an_And_expression_with_different_values
	{
		[Test]
		public void A_null_value_in_the_object_should_not_match()
		{
			Member member = new Member();
			member.Attempts = 7;

			ISpecification<Member> specification = new Specification<Member>()
				.Equal("Username", "joey")
				.And
				.Equal("Attempts", 9);

			Assert.That(specification.IsSatisfiedBy(member), Is.False);
		}

		[Test]
		public void A_null_value_on_either_side_should_not_match()
		{
			Member member = new Member();
			member.Username = "joey";
			member.Attempts = 7;

			ISpecification<Member> specification = new Specification<Member>()
				.Equal<string>("Username", null)
				.And
				.Equal("Attempts", 9);

			Assert.That(specification.IsSatisfiedBy(member), Is.False);
		}

		[Test]
		public void An_different_left_specification_should_not_match()
		{
			Member member = new Member();
			member.Username = "joey";
			member.Attempts = 7;

			ISpecification<Member> specification = new Specification<Member>()
				.Equal("Username", "jimmy")
				.And
				.Equal("Attempts", 7);

			Assert.That(specification.IsSatisfiedBy(member), Is.False);
		}

		[Test]
		public void An_different_right_specification_should_not_match()
		{
			Member member = new Member();
			member.Username = "joey";
			member.Attempts = 7;

			ISpecification<Member> specification = new Specification<Member>()
				.Equal("Username", "joey")
				.And
				.Equal("Attempts", 9);

			Assert.That(specification.IsSatisfiedBy(member), Is.False);
		}
	}

	[TestFixture]
	public class When_specifying_an_And_expression_with_matching_values : Specification
	{
		[Test]
		public void All_three_expressions_should_match()
		{
			Member member = new Member();
			member.Username = "joey";
			member.Attempts = 7;
			member.EmailAddress = "joey@domain.com";

			ISpecification<Member> specification = new Specification<Member>()
				.Equal("Username", "joey")
				.And
				.Equal("Attempts", 7)
				.And
				.Equal("EmailAddress", "joey@domain.com");

			Assert.That(specification.IsSatisfiedBy(member), Is.True);
		}

		[Test]
		public void Both_the_left_and_right_specification_should_match()
		{
			Member member = new Member();
			member.Username = "joey";
			member.Attempts = 7;

			ISpecification<Member> specification = new Specification<Member>()
				.Equal("Username", "joey")
				.And
				.Equal("Attempts", 7);

			Assert.That(specification.IsSatisfiedBy(member), Is.True);
		}
	}

	[TestFixture]
	public class When_specifying_an_Or_expression_with_only_one_matching_value : Specification
	{
		[Test]
		public void The_expression_should_match()
		{
			Member member = new Member();
			member.Username = "joey";
			member.Attempts = 7;

			ISpecification<Member> specification = new Specification<Member>()
				.Equal("Username", "joey")
				.Or
				.Equal("Attempts", 9);

			Assert.That(specification.IsSatisfiedBy(member), Is.True);
		}
	}

	[TestFixture]
	public class When_specifying_an_Or_expression_with_only_no_matching_values : Specification
	{
		[Test]
		public void The_expression_should_match()
		{
			Member member = new Member();
			member.Username = "joey";
			member.Attempts = 7;

			ISpecification<Member> specification = new Specification<Member>()
				.Equal("Username", "jimmy")
				.Or
				.Equal("Attempts", 9);

			Assert.That(specification.IsSatisfiedBy(member), Is.False);
		}
	}
}
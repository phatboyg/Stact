namespace Magnum.Core.Tests
{
	using System.Collections.Generic;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Validation;
	using Validators;

	[TestFixture]
	public class When_validating_an_object : Specification
	{
		internal class MemberValidator : IValidator<Member>
		{
			public bool IsValid(Member entity, out IEnumerable<IViolation> violations)
			{
				List<IViolation> violationList = new List<IViolation>();

				if (string.IsNullOrEmpty(entity.EmailAddress))
					violationList.Add(new EmptyValueViolation("EmailAddress"));

				violations = violationList;

				return violationList.Count == 0;
			}
		}

		[Test]
		public void No_violations_should_be_returned_if_the_object_is_valid()
		{
			IValidator<Member> validator = new MemberValidator();

			Member member = new Member();

			IEnumerable<IViolation> violations;

			member.Validate(validator, out violations);

			Assert.That(validator.IsValid(member, out violations), Is.False);

			List<IViolation> violationList = new List<IViolation>(violations);

			Assert.That(violationList.Count, Is.GreaterThan(0));

			Assert.That(violationList[0].ToString(), Is.EqualTo("EmailAddress must contain a value"));
		}
	}
}
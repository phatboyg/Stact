namespace Magnum.Core.Tests.Validators
{
	using System.Collections.Generic;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Validation;

	[TestFixture]
	public class When_the_EmailAddressPropertyValidator_is_used_on_an_entity : Specification
	{
		[Test]
		public void An_EmptyPropertyViolation_should_be_returned_if_the_property_is_empty()
		{
			Member member = new Member();
			member.EmailAddress = string.Empty;

			IValidator<object> validator = new EmailAddressValidator(typeof (Member).GetProperty("EmailAddress"));

			IEnumerable<IViolation> violations;

			Assert.That(validator.IsValid(member, out violations), Is.False);

			List<IViolation> violationList = new List<IViolation>(violations);

			Assert.That(violationList.Count, Is.EqualTo(1));

			Assert.That(violationList[0], Is.TypeOf((typeof (EmptyValueViolation))));
		}

		[Test]
		public void An_InvalidFormatViolation_should_be_returned_if_the_email_address_is_invalid()
		{
			Member member = new Member();
			member.EmailAddress = "joe";

			IValidator<object> validator = new EmailAddressValidator(typeof(Member).GetProperty("EmailAddress"));

			IEnumerable<IViolation> violations;

			Assert.That(validator.IsValid(member, out violations), Is.False);

			List<IViolation> violationList = new List<IViolation>(violations);

			Assert.That(violationList.Count, Is.EqualTo(1));

			Assert.That(violationList[0], Is.TypeOf((typeof(InvalidFormatViolation))));
		}

		[Test]
		public void A_valid_email_address_should_return_no_violations()
		{
			Member member = new Member();
			member.EmailAddress = "joe@domain.com";

			IValidator<object> validator = new EmailAddressValidator(typeof(Member).GetProperty("EmailAddress"));

			IEnumerable<IViolation> violations;

			Assert.That(validator.IsValid(member, out violations), Is.True);

			List<IViolation> violationList = new List<IViolation>(violations);

			Assert.That(violationList.Count, Is.EqualTo(0));
		}

	}
}
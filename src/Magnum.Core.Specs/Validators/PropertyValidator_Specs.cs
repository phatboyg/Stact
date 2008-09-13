namespace Magnum.Core.Tests.Validators
{
	using System.Collections.Generic;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Validation;

	[TestFixture]
	public class When_validating_a_property
	{
		[Test]
		public void A_property_that_is_too_short_should_return_a_format_violation()
		{
			Member member = new Member();
			member.Username = "joe";

			IValidator<object> validator = new PropertyValidator(typeof (Member).GetProperty("Username"));

			IEnumerable<IViolation> violations;

			Assert.That(validator.IsValid(member, out violations), Is.False);

			List<IViolation> violationList = new List<IViolation>(violations);

			Assert.That(violationList.Count, Is.EqualTo(1));

			Assert.That(violationList[0], Is.TypeOf((typeof (InvalidFormatViolation))));
		}

		[Test]
		public void A_property_that_is_too_long_should_return_a_format_violation()
		{
			Member member = new Member();
			member.Username = "joewantstobeasuperherobutitisntgoingtohappenhere";

			IValidator<object> validator = new PropertyValidator(typeof(Member).GetProperty("Username"));

			IEnumerable<IViolation> violations;

			Assert.That(validator.IsValid(member, out violations), Is.False);

			List<IViolation> violationList = new List<IViolation>(violations);

			Assert.That(violationList.Count, Is.EqualTo(1));

			Assert.That(violationList[0], Is.TypeOf((typeof(InvalidFormatViolation))));
		}

		[Test]
		public void A_property_that_is_valid_should_not_return_any_violations()
		{
			Member member = new Member();
			member.Username = "joem";

			IValidator<object> validator = new PropertyValidator(typeof(Member).GetProperty("Username"));

			IEnumerable<IViolation> violations;

			Assert.That(validator.IsValid(member, out violations), Is.True);

			List<IViolation> violationList = new List<IViolation>(violations);

			Assert.That(violationList.Count, Is.EqualTo(0));
		}

	}
}
namespace Magnum.Core.Tests.Validators
{
	using System.Collections.Generic;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Validation;

	[TestFixture]
	public class When_the_EntityValidator_is_used_on_an_object : Specification
	{
		[Test]
		public void The_attributes_on_the_properties_should_be_used_for_validation()
		{
			IValidator<Member> validator = new EntityValidator<Member>();

			Member member = new Member();

			IEnumerable<IViolation> violations;

			member.Validate(validator, out violations);

			Assert.That(validator.IsValid(member, out violations), Is.False);

			List<IViolation> violationList = new List<IViolation>(violations);

			Assert.That(violationList.Count, Is.EqualTo(2));

			Assert.That(violationList[0], Is.TypeOf(typeof(NullValueViolation)));
			Assert.That(violationList[1], Is.TypeOf(typeof(NullValueViolation)));
		}
	}
}
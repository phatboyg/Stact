namespace Magnum.Core.Tests.Validators
{
	using System.Collections.Generic;
	using Validation;

	public class Member : IValidatable<Member>
	{
		private int _attempts;
		private string _emailAddress;
		private string _username;

		[ValidateEmailAddress]
		public string EmailAddress
		{
			get { return _emailAddress; }
			set { _emailAddress = value; }
		}

		[Validate(MinLength = 4, MaxLength = 30)]
		public string Username
		{
			get { return _username; }
			set { _username = value; }
		}

		public int Attempts
		{
			get { return _attempts; }
			set { _attempts = value; }
		}


		public bool Validate(IValidator<Member> validator, out IEnumerable<IViolation> violations)
		{
			return validator.IsValid(this, out violations);
		}
	}
}
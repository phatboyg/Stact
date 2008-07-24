namespace Magnum.Core.Validation
{
	using System.Collections.Generic;
	using System.Reflection;
	using System.Text.RegularExpressions;

	public class EmailAddressValidator : PropertyValidator
	{
		private static readonly Regex _regex;

		static EmailAddressValidator()
		{
			_regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
		}

		public EmailAddressValidator(PropertyInfo propertyInfo, ValidateAttribute attribute)
			: base(propertyInfo, attribute)
		{
		}

		public EmailAddressValidator(PropertyInfo info)
			: base(info)
		{
		}

		protected override bool ValidateProperty(object obj, List<IViolation> violationList)
		{
			if (base.ValidateProperty(obj, violationList) == false)
				return false;

			if (obj != null)
			{
				string value = obj.ToString();
				if (!_regex.IsMatch(value))
				{
					violationList.Add(new InvalidFormatViolation(Info.Name, "is not a properly formatted email address"));
					return false;
				}
			}

			return true;
		}
	}
}
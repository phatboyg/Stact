namespace Magnum.Core.Validation
{
	using System.Collections.Generic;
	using System.Globalization;
	using System.Reflection;
	using CheckHelpers;

	public class PropertyValidator : IValidator<object>
	{
		private readonly ValidateAttribute _attribute;
		private readonly PropertyInfo _info;

		public PropertyValidator(PropertyInfo info)
		{
			_info = info;

			object[] attributes = info.GetCustomAttributes(typeof (ValidateAttribute), true);

			Check.That(attributes, Is.Not.Null);

			_attribute = attributes[0] as ValidateAttribute;
		}

		public PropertyValidator(PropertyInfo info, ValidateAttribute attribute)
		{
			_info = info;
			_attribute = attribute;
		}

		public PropertyInfo Info
		{
			get { return _info; }
		}

		public bool IsValid(object entity, out IEnumerable<IViolation> violations)
		{
			List<IViolation> violationList = new List<IViolation>();

			object obj = _info.GetValue(entity, BindingFlags.Default, null, null, CultureInfo.InvariantCulture);

			bool valid = ValidateProperty(obj, violationList);

			violations = violationList;
			return valid;
		}

		protected virtual bool ValidateProperty(object obj, List<IViolation> violationList)
		{
			if (obj == null)
			{
				if (_attribute.AllowNull == false)
				{
					violationList.Add(new NullValueViolation(Info.Name));
					return false;
				}
			}
			else
			{
				if(_info.PropertyType == typeof(string))
				{
					string value = (string)obj;
					if (value.Length == 0 && _attribute.AllowEmpty == false)
					{
						violationList.Add(new EmptyValueViolation(Info.Name));
						return false;
					}

					if( value.Length <_attribute.MinLength )
					{
						violationList.Add(new InvalidFormatViolation(Info.Name, "must be at least " + _attribute.MinLength + " characters"));
						return false;
					}

					if (value.Length > _attribute.MaxLength)
					{
						violationList.Add(new InvalidFormatViolation(Info.Name, "must be no more than " + _attribute.MaxLength + " characters"));
						return false;
					}
				}
			}

			return true;
		}
	}
}
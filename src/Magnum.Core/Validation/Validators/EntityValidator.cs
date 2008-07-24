namespace Magnum.Core.Validation
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	public class EntityValidator<TEntity> : IValidator<TEntity>
	{
		private static readonly Type _typeOfEntity;
		private static readonly List<IValidator<object>> _validators = new List<IValidator<object>>();

		static EntityValidator()
		{
			_typeOfEntity = typeof (TEntity);

			PropertyInfo[] properties = _typeOfEntity.GetProperties();
			foreach (PropertyInfo info in properties)
			{
				object[] attributes = info.GetCustomAttributes(typeof (ValidateAttribute), true);
				foreach (ValidateAttribute attribute in attributes)
				{
					if (attribute is ValidateEmailAddressAttribute)
						_validators.Add(new EmailAddressValidator(info, attribute));
					else
						_validators.Add(new PropertyValidator(info, attribute));
				}
			}
		}

		public bool IsValid(TEntity entity, out IEnumerable<IViolation> violations)
		{
			List<IViolation> violationList = new List<IViolation>();

			bool valid = true;
			foreach (IValidator<object> validator in _validators)
			{
				IEnumerable<IViolation> innerViolations;
				if (validator.IsValid(entity, out innerViolations) == false)
				{
					valid = false;
				}

				violationList.AddRange(innerViolations);
			}

			violations = violationList;

			return valid;
		}
	}
}
namespace Magnum.Core.Validation
{
	using System;

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ValidateEmailAddressAttribute :
		ValidateAttribute
	{
	}
}
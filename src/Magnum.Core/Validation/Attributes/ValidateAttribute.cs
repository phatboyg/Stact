namespace Magnum.Core.Validation
{
	using System;

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ValidateAttribute : Attribute
	{
		private bool _allowEmpty = false;
		private bool _allowNull = false;
		private int _maxLength = int.MaxValue;
		private int _minLength = int.MinValue;

		public bool AllowNull
		{
			get { return _allowNull; }
			set { _allowNull = value; }
		}

		public bool AllowEmpty
		{
			get { return _allowEmpty; }
			set { _allowEmpty = value; }
		}

		public int MaxLength
		{
			get { return _maxLength; }
			set { _maxLength = value; }
		}

		public int MinLength
		{
			get { return _minLength; }
			set { _minLength = value; }
		}
	}
}
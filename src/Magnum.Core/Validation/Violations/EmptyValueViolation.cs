namespace Magnum.Core.Validation
{
	public class EmptyValueViolation : IViolation
	{
		private readonly string _propertyName;

		public EmptyValueViolation(string propertyName)
		{
			_propertyName = propertyName;
		}

		public override string ToString()
		{
			return string.Format("{0} must contain a value", _propertyName);
		}
	}
}
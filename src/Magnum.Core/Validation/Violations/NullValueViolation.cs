namespace Magnum.Core.Validation
{
	public class NullValueViolation : IViolation
	{
		private readonly string _propertyName;

		public NullValueViolation(string propertyName)
		{
			_propertyName = propertyName;
		}

		public override string ToString()
		{
			return string.Format("{0} cannot be null", _propertyName);
		}
	}
}
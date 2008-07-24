namespace Magnum.Core.Validation
{
	public class InvalidFormatViolation : IViolation
	{
		private readonly string _propertyName;
		private readonly string _message;


		public InvalidFormatViolation(string propertyName)
		{
			_propertyName = propertyName;
			_message = "is improperly formatted";
		}

		public InvalidFormatViolation(string propertyName, string message)
		{
			_propertyName = propertyName;
			_message = message;
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", _propertyName, _message);
		}
	}
}
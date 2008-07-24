namespace Magnum.Core.CheckHelpers
{
	public class NullCheckExpression : ICheckExpression
	{
		private readonly bool _nullExpected;

		public NullCheckExpression(bool nullExpected)
		{
			_nullExpected = nullExpected;
		}

		public bool Validate(object obj)
		{
			if (obj == null)
				return _nullExpected;
			else
				return !_nullExpected;
		}

		public override string ToString()
		{
			return _nullExpected ? "Expression must be null" : "Expression must not be null";
		}
	}
}
namespace Magnum.Core.CheckHelpers
{
	public static class Is
	{
		public static CheckConstraint Not
		{
			get { return NewConstraint(false); }
		}

		public static ICheckExpression Empty
		{
			get { return new CheckConstraint(true).Empty; }
		}

		private static CheckConstraint NewConstraint(bool isTrue)
		{
			return new CheckConstraint(isTrue);
		}
	}

	public class CheckConstraint
	{
		private readonly bool _isTrue;

		public CheckConstraint(bool isTrue)
		{
			_isTrue = isTrue;
		}

		public ICheckExpression Null
		{
			get { return new NullCheckExpression(_isTrue); }
		}

		public ICheckExpression Empty
		{
			get { return new EmptyCheckExpression(_isTrue); }
		}
	}
}
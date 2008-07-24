namespace Magnum.Core.CheckHelpers
{
	public class CheckExpression
	{
		public ICheckExpression Null
		{
			get { return new NullCheckExpression(false); }
		}

		public ICheckExpression Empty
		{
			get { return new EmptyCheckExpression(false); }
		}
	}
}
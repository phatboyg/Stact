namespace Magnum.Core.CheckHelpers
{
	internal class EmptyCheckExpression : ICheckExpression
	{
		private readonly bool _emptyExpected;

		public EmptyCheckExpression(bool emptyExpected)
		{
			_emptyExpected = emptyExpected;
		}

		public bool Validate(object obj)
		{
			if (obj == null)
				return _emptyExpected;
			else
			{
				if (obj is string)
				{
					if (string.IsNullOrEmpty((string) obj))
						return _emptyExpected;
					else
						return !_emptyExpected;
				}

				return !_emptyExpected;
			}
		}
	}
}
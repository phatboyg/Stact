namespace Magnum.Core.CheckHelpers
{
	using System;

	public class CheckException : Exception
	{
		public CheckException(string message)
			: base(message)
		{

		}
	}
}
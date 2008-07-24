namespace Magnum.Core.CheckHelpers
{
	public interface ICheckExpression
	{
		bool Validate(object obj);
	}
}
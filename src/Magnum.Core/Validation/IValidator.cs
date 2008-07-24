namespace Magnum.Core.Validation
{
	using System.Collections.Generic;

	public interface IValidator<TEntity>
	{
		bool IsValid(TEntity entity, out IEnumerable<IViolation> violations);
	}
}
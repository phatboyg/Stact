namespace Magnum.Core.Validation
{
	using System.Collections.Generic;

	public interface IValidatable<TEntity>
	{
		bool Validate(IValidator<TEntity> validator, out IEnumerable<IViolation> violations);
	}
}
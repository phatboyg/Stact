namespace Magnum.Core.Specification
{
	public interface ISpecification
	{
		bool IsSatisfiedBy(object obj);
	}

	public interface ISpecification<TCandidate>
	{
		bool IsSatisfiedBy(TCandidate candidate);
	}

	public interface ISpecificationBuilder<TCandidate> : ISpecification<TCandidate>
	{
		ISpecificationBuilder<TCandidate> And { get; }

		ISpecificationBuilder<TCandidate> Or { get; }

		ISpecificationBuilder<TCandidate> Equal<TValue>(string propertyName, TValue value);
	}
}
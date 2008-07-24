namespace Magnum.Core.Specification
{
	using System;

	public class PredicateSpecification<TCandidate> : ISpecification<TCandidate>
	{
		private readonly Predicate<TCandidate> _predicate;

		public PredicateSpecification(Predicate<TCandidate> predicate)
		{
			_predicate = predicate;
		}

		public bool IsSatisfiedBy(TCandidate candidate)
		{
			return _predicate(candidate);
		}
	}
}
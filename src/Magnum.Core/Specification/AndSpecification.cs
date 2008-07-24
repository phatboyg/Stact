namespace Magnum.Core.Specification
{
	using CheckHelpers;

	public class AndSpecification<TCandidate> : CompositeSpecification<TCandidate>
	{
		private readonly ISpecification<TCandidate> _left;
		private ISpecification<TCandidate> _right;

		public AndSpecification(ISpecification<TCandidate> left)
		{
			_left = left;
		}

		public override bool IsSatisfiedBy(TCandidate candidate)
		{
			Check.That(_right, Is.Not.Null);

			return _left.IsSatisfiedBy(candidate) && _right.IsSatisfiedBy(candidate);
		}

		public override ISpecificationBuilder<TCandidate> Equal<TValue>(string propertyName, TValue value)
		{
			_right = new EqualSpecification<TCandidate, TValue>(propertyName, value);

			return this;
		}
	}
}
namespace Magnum.Core.Specification
{
	using System;

	public abstract class CompositeSpecification<TCandidate> : ISpecificationBuilder<TCandidate>
	{
		public abstract bool IsSatisfiedBy(TCandidate candidate);

		public virtual  ISpecificationBuilder<TCandidate> And
		{
			get { return new AndSpecification<TCandidate>(this); }
		}

		public ISpecificationBuilder<TCandidate> Or
		{
			get { return new OrSpecification<TCandidate>(this); }
		}

		public virtual ISpecificationBuilder<TCandidate> Equal<TValue>(string propertyName, TValue value)
		{
			return new EqualSpecification<TCandidate, TValue>(propertyName, value);
		}
	}

	public class Specification<TCandidate> : CompositeSpecification<TCandidate>
	{
		private static readonly ISpecification<TCandidate> _all = new AllSpecification<TCandidate>();

		public override ISpecificationBuilder<TCandidate> And
		{
			get { throw new SpecificationBuilderException("Ands cannot be root specifications"); }
		}

		public static ISpecification<TCandidate> All
		{
			get { return _all; }
		}

		public override bool IsSatisfiedBy(TCandidate candidate)
		{
			throw new NotImplementedException();
		}
	}
}
namespace Magnum.Core.Specification
{
	public class EqualSpecification<TCandidate, TValue> :
		PropertySpecification<TCandidate, TValue>
	{
		public EqualSpecification(string propertyName, TValue value)
			: base(propertyName, value)
		{
		}

		public override bool IsSatisfiedBy(TCandidate candidate)
		{
			TValue actual = GetCandidateValue(candidate);

			if (typeof (TValue).IsValueType)
				return actual.Equals(Value);

			if (Equals(actual, default(TValue)) && Equals(Value, default(TValue)))
				return true;

			if (Equals(actual, default(TValue)) || Equals(Value, default(TValue)))
				return false;

			return actual.Equals(Value);
		}
	}
}
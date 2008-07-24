namespace Magnum.Core.Specification
{
	/// <summary>
	/// A global specification that matches everything passed through it
	/// </summary>
	/// <typeparam name="TCandidate"></typeparam>
	public class AllSpecification<TCandidate> : ISpecification<TCandidate>
	{
		public bool IsSatisfiedBy(TCandidate candidate)
		{
			return true;
		}
	}
}
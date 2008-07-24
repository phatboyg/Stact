namespace Magnum.Core.Specification
{
	public abstract class PropertySpecification<TCandidate, TValue> :
		CompositeSpecification<TCandidate>
	{
		private readonly string[] _parsedPropertyName;
		private readonly string _propertyName;
		private readonly TValue _value;

		protected PropertySpecification(string propertyName, TValue value)
		{
			_propertyName = propertyName;
			_value = value;
			_parsedPropertyName = _propertyName.Split('.');
		}

		private string[] ParsedPropertyName
		{
			get { return _parsedPropertyName; }
		}

		public TValue Value
		{
			get { return _value; }
		}

		protected object GetCandidateObject(TCandidate candidate)
		{
			object candidateObject = candidate;
			foreach (string name in ParsedPropertyName)
			{
				candidateObject = candidateObject.GetType().GetProperty(name).GetValue(candidateObject, null);
			}

			return candidateObject;
		}

		protected string GetCandidateString(TCandidate candidate)
		{
			return GetCandidateObject(candidate).ToString();
		}

		protected TValue GetCandidateValue(TCandidate candidate)
		{
			return (TValue) GetCandidateObject(candidate);
		}
	}
}
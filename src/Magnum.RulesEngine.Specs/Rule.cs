namespace Magnum.RulesEngine.Specs
{
	public class Rule
	{
		
	}

	public class RuleViolation
	{
		private readonly string _message;

		public RuleViolation(string message)
		{
			_message = message;
		}

		public string Message
		{
			get { return _message; }
		}
	}





	internal interface IRuleDefinition
	{
	}
}
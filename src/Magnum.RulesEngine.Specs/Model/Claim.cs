namespace Magnum.RulesEngine.Specs.Model
{
	using System.Collections.Generic;

	public class Claim
	{
		public string Id { get; set; }

		public IList<Payer> Payers { get; set; }
	}
}
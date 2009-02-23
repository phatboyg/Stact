namespace Magnum.ActorModel.WebSpecs.Simple
{
	using System;

	public class SimpleRequest
	{
		public Action<SimpleResponse> Reply { get; set; }
	}
}
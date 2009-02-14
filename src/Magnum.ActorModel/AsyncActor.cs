namespace Magnum.ActorModel
{
	using System;

	public interface AsyncActor
	{
		IAsyncResult Begin(AsyncCallback callback, object state);
	}
}
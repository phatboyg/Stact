namespace Magnum.ActorModel
{
	using System;
	using System.Web;

	public interface AsyncHttpActor :
		Actor
	{
		IAsyncResult BeginAction(HttpContext context, AsyncCallback callback, object state);
	}
}
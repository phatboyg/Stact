namespace Magnum.ActorModel.WebSpecs
{
	using System;
	using System.Web;
	using Threading;

	public class HttpHandlerAsyncResult :
		AsyncResult
	{
		public HttpHandlerAsyncResult(HttpContext context, AsyncCallback callback, object state) :
			base(callback, state)
		{
			Context = context;
		}

		public HttpContext Context { get; set; }
	}
}
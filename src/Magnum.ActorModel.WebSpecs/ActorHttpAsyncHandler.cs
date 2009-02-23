namespace Magnum.ActorModel.WebSpecs
{
	using System;
	using System.Web;
	using StructureMap;



	public class HttpAsyncActorHandler :
		IHttpAsyncHandler
	{
		private readonly Func<AsyncHttpActor> _getInstance;

		public HttpAsyncActorHandler(Func<AsyncHttpActor> getInstance)
		{
			_getInstance = getInstance;
		}

		public void ProcessRequest(HttpContext context)
		{
			throw new InvalidOperationException("This should not be called since we are an asynchronous handler");
		}

		public bool IsReusable
		{
			get { return true; }
		}

		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			AsyncHttpActor actor = _getInstance();

			return actor.BeginAction(context, cb, extraData);
		}

		public void EndProcessRequest(IAsyncResult result)
		{
		}
	}



//	public class ActorHttpAsyncHandler<T> :
//		IHttpAsyncHandler
//		where T : AsyncActor
//	{
//		private readonly CommandQueue _queue;
//
//		public ActorHttpAsyncHandler(CommandQueue queue)
//		{
//			_queue = queue;
//		}
//
//		public void ProcessRequest(HttpContext context)
//		{
//			throw new InvalidOperationException("This should not be called since we are an asynchronous handler");
//		}
//
//		public bool IsReusable
//		{
//			get { return true; }
//		}
//
//		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
//		{
//			var actor = ObjectFactory.With(context).With(_queue).GetInstance<T>();
//
//			return actor.Begin(cb, extraData);
//		}
//
//		public void EndProcessRequest(IAsyncResult result)
//		{
//		}
//	}
}
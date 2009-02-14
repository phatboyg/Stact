namespace Magnum.ActorModel.WebSpecs
{
	using System;
	using System.Web;
	using CommandQueues;
	using StructureMap;

	public class ActorHttpAsyncHandler<T> :
		IHttpAsyncHandler
		where T : AsyncActor
	{
		private readonly CommandQueue _queue;

		public ActorHttpAsyncHandler()
		{
			_queue = ObjectFactory.GetInstance<ThreadPoolCommandQueue>();
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
			var actor = ObjectFactory.With(context).With(_queue).GetInstance<T>();

			return actor.Begin(cb, extraData);
		}

		public void EndProcessRequest(IAsyncResult result)
		{
		}
	}
}
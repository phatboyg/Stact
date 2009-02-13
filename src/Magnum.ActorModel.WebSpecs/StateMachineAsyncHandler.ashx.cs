namespace Magnum.ActorModel.WebSpecs
{
	using System;
	using System.Web;
	using StructureMap;

	public class StateMachineAsyncHandler :
		IHttpAsyncHandler
	{
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
			var actor = ObjectFactory.With(context).GetInstance<SimpleRequestActor>();

			//var actor = new SimpleRequestActor(context, _requestChannel, _responseChannel, _queue);


			return actor.Begin(cb, extraData);
		}

		public void EndProcessRequest(IAsyncResult result)
		{
		}
	}
}
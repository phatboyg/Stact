namespace Magnum.ActorModel.WebSpecs
{
	using System;
	using System.Web;
	using System.Web.Routing;
	using StructureMap;

	public class ActorRouteHandler :
		IRouteHandler
	{
		private readonly Type _type;

		public ActorRouteHandler(Type type)
		{
			_type = type;
		}

		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			var handler = ObjectFactory
				.With<Func<AsyncHttpActor>>(() => (AsyncHttpActor)ObjectFactory.GetInstance(_type))
				.GetInstance<HttpAsyncActorHandler>();

			return handler;
		}
	}
}
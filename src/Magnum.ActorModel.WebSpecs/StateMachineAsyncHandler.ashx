<%@ WebHandler Language="C#" Class="StateMachineAsyncHandler" %>

using Magnum.ActorModel.WebSpecs;

public class StateMachineAsyncHandler :
	ActorHttpAsyncHandler<SimpleRequestActor>
{
}

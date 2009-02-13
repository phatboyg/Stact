namespace Magnum.ActorModel
{
	using StateMachine;

	public class StateDrivenActor<T> :
		StateMachine<T>
		where T : StateDrivenActor<T>
	{
		static StateDrivenActor()
		{
			Define(() => { });
		}

//		public static State Initial { get; set; }
//		public static State Completed { get; set; }
//
//		public static State Waiting { get; set; }
//
//		public static Event Started { get; set; }
	}
}
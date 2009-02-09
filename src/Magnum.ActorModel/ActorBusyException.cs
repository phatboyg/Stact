namespace Magnum.ActorModel
{
	using System;
	using System.Runtime.Serialization;

	public class ActorBusyException : Exception
	{
		public ActorBusyException()
		{
		}

		public ActorBusyException(int queueDepth)
			: this("This actor is too busy to accept new commands (" + queueDepth + ")")
		{
		}

		public ActorBusyException(string message) :
			base(message)
		{
		}

		public ActorBusyException(string message, Exception innerException) :
			base(message, innerException)
		{
		}

		protected ActorBusyException(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}
	}
}
namespace Magnum.ActorModel.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	public class QueueFullException : Exception
	{
		public QueueFullException()
		{
		}

		public QueueFullException(int queueDepth)
			: this("This actor is too busy to accept new commands (" + queueDepth + ")")
		{
		}

		public QueueFullException(string message) :
			base(message)
		{
		}

		public QueueFullException(string message, Exception innerException) :
			base(message, innerException)
		{
		}

		protected QueueFullException(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}
	}
}
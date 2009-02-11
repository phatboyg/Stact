namespace Magnum.ActorModel.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	public class QueueFullException : Exception
	{
		private readonly int _queueDepth;

		public QueueFullException()
		{
		}

		public QueueFullException(int queueDepth)
			: this("The queue is full and cannot accept new commands (" + queueDepth + ")")
		{
			_queueDepth = queueDepth;
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

		public int Depth
		{
			get { return _queueDepth; }
		}
	}
}
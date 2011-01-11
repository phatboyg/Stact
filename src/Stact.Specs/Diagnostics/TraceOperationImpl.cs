namespace Stact.Specs.Diagnostics
{
	using System;
	using System.Diagnostics;
	using Magnum;


	public class TraceOperationImpl : 
		TraceOperation
	{
		readonly string _operationKey;
		TraceChannel _channel;
		Guid _id;
		DateTime _startTime;
		Stopwatch _stopwatch;

		public TraceOperationImpl(TraceChannel channel, string operationKey)
		{
			_id = CombGuid.Generate();
			_startTime = SystemUtil.UtcNow;
			_stopwatch = Stopwatch.StartNew();
			_channel = channel;
			_operationKey = operationKey;

			_channel.Send<TraceOperationStarted>(new TraceOperationStartedImpl
				{
					Id = _id,
					StartTime = _startTime,
				});
		}

		public void Dispose()
		{
			_stopwatch.Stop();

			_channel.Send<TraceOperationComplete>(new TraceOperationCompleteImpl
				{
					Id = _id,
					Duration = _stopwatch.ElapsedMilliseconds,
				});
		}

		public void Message(int level, string message)
		{
			long timestamp = _stopwatch.ElapsedMilliseconds;

			_channel.Send<TraceOperationMessage>(new TraceOperationMessageImpl
				{
					Id = _id,
					Level = level,
					Timestamp = timestamp,
					Message = message,
				});
		}
	}
}
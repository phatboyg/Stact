namespace Magnum.ActorModel.Specs.Demos.Scoreboard
{
	using System.Diagnostics;
	using Channels;
	using Messages;

	public class HighScoreBoard :
		IStartable
	{
		private readonly CommandQueue _queue;
		private readonly Channel<UpdateHighScore> _uhs;

		public HighScoreBoard(CommandQueue queue, Channel<UpdateHighScore> uhs) 
		{
			_queue = queue;
			_uhs = uhs;
		}

		public void Consume(UpdateHighScore message)
		{
			Trace.WriteLine("High score received for " + message.Score);
		}

		public void Start()
		{
			_uhs.Subscribe(_queue, Consume);

			Trace.WriteLine("Started High Score Board");
		}
	}
}
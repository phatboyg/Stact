namespace Magnum.ActorModel.Specs.Demos.Scoreboard.Messages
{
	using System;

	public class UpdateHighScore
	{
		public Guid PlayerId { get; set; }
		public DateTime DateAchieved { get; set; }
		public long Score { get; set; }
	}
}
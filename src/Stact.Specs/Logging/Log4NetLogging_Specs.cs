namespace Stact.Specs.Logging
{
	using System.IO;
	using Stact.Logging;
	using NUnit.Framework;
	using TestFramework;


	[TestFixture]
	public class Logging_with_log4net
	{
		[Given]
		public void A_log4net_logger_is_configured()
		{
			var fileInfo = new FileInfo("Logging/log4net.xml");

			fileInfo.Exists.ShouldBeTrue("No log4net.xml found in test directory");

			Log4NetLogger.Configure(fileInfo);
		}

		[Test]
		public void Should_output_the_proper_messages()
		{
			Logger.GetLogger<int>().Debug(x => x.Write("Hello again, my friend"));
		}

		[Test]
		public void Should_not_output_invisible_messages()
		{
			Logger.GetLogger<Logging_with_log4net>().Debug(x => x.Write("Hello again, my friend"));
		}
	}
}

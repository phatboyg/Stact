// Copyright 2007-2010 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Magnum.Logging
{
	using System;
	using System.IO;
	using Channels;
	using Collections;
	using Fibers;
	using ForLog4Net;
	using log4net;
	using log4net.Config;
	using Messages;


	public static class Log4NetLogger
	{
		public static void Configure()
		{
			var listeners = new Cache<string, Log4NetLogInstance>(key => new Log4NetLogInstance(new SynchronousFiber(), key));

			Logger.LogChannel.Connect(x =>
				{
					x.AddConsumerOf<DebugLogMessage>()
						.UsingInstance().Of<Log4NetLogInstance>()
						.DistributedBy(m => m.Source)
						.HandleOnCallingThread()
						.ObtainedBy(m => listeners[m.Source])
						.OnChannel(c => c.DebugChannel);

					x.AddConsumerOf<InfoLogMessage>()
						.UsingInstance().Of<Log4NetLogInstance>()
						.DistributedBy(m => m.Source)
						.HandleOnCallingThread()
						.ObtainedBy(m => listeners[m.Source])
						.OnChannel(c => c.InfoChannel);

					x.AddConsumerOf<WarnLogMessage>()
						.UsingInstance().Of<Log4NetLogInstance>()
						.DistributedBy(m => m.Source)
						.HandleOnCallingThread()
						.ObtainedBy(m => listeners[m.Source])
						.OnChannel(c => c.WarnChannel);

					x.AddConsumerOf<ErrorLogMessage>()
						.UsingInstance().Of<Log4NetLogInstance>()
						.DistributedBy(m => m.Source)
						.HandleOnCallingThread()
						.ObtainedBy(m => listeners[m.Source])
						.OnChannel(c => c.ErrorChannel);

					x.AddConsumerOf<FatalLogMessage>()
						.UsingInstance().Of<Log4NetLogInstance>()
						.DistributedBy(m => m.Source)
						.HandleOnCallingThread()
						.ObtainedBy(m => listeners[m.Source])
						.OnChannel(c => c.FatalChannel);
				});
		}

		public static void Configure(FileInfo fileInfo)
		{
			try
			{
				XmlConfigurator.Configure(fileInfo);

				LogManager.GetLogger(typeof(Log4NetLogger));

				Configure();
			}
			catch (Exception ex)
			{
				throw new LoggerException("The Log4Net assembly is not referenced or could not be initialized", ex);
			}
		}
	}
}
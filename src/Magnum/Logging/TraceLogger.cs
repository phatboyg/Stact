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
	using System.Diagnostics;
	using Channels;
	using Messages;


	public static class TraceLogger
	{
		static void WriteInformation(LogMessage message)
		{
			Trace.TraceInformation(message.Message);
			if (message.Exception != null)
				Trace.TraceInformation(message.Exception);
		}

		static void WriteError(LogMessage message)
		{
			Trace.TraceError(message.Message);
			if (message.Exception != null)
				Trace.TraceError(message.Exception);
		}

		static void WriteWarning(LogMessage message)
		{
			Trace.TraceWarning(message.Message);
			if (message.Exception != null)
				Trace.TraceWarning(message.Exception);
		}

		public static void Configure()
		{
			Configure(LogLevel.Info);
		}

		public static void Configure(LogLevel logLevel)
		{
			Logger.LogChannel.Connect(x =>
				{
					x.AddConsumerOf<DebugLogMessage>()
						.Where(m => m.Level.IsEnabledForLevel(logLevel))
						.HandleOnCallingThread()
						.UsingConsumer(WriteInformation)
						.HandleOnCallingThread();
					x.AddConsumerOf<InfoLogMessage>()
						.Where(m => m.Level.IsEnabledForLevel(logLevel))
						.HandleOnCallingThread()
						.UsingConsumer(WriteInformation)
						.HandleOnCallingThread();
					x.AddConsumerOf<WarnLogMessage>()
						.Where(m => m.Level.IsEnabledForLevel(logLevel))
						.HandleOnCallingThread()
						.UsingConsumer(WriteWarning)
						.HandleOnCallingThread();
					x.AddConsumerOf<ErrorLogMessage>()
						.Where(m => m.Level.IsEnabledForLevel(logLevel))
						.HandleOnCallingThread()
						.UsingConsumer(WriteError)
						.HandleOnCallingThread();
					x.AddConsumerOf<FatalLogMessage>()
						.Where(m => m.Level.IsEnabledForLevel(logLevel))
						.HandleOnCallingThread()
						.UsingConsumer(WriteError)
						.HandleOnCallingThread();
				});
		}
	}
}
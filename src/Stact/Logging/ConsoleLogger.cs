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
namespace Stact.Logging
{
	using System;
	using Channels;
	using Fibers;
	using Messages;


	public static class ConsoleLogger
	{
		static void WriteLine(LogMessage message, ConsoleColor color)
		{
			Console.ForegroundColor = color;

			Console.WriteLine(message.Message);
			if (message.Exception != null)
				Console.WriteLine(message.Exception);
			Console.ResetColor();
		}


		public static void Configure()
		{
			Configure(LogLevel.Info);
		}

		public static void Configure(LogLevel logLevel)
		{
			Fiber consoleFiber = new ThreadPoolFiber();

			Logger.LogChannel.Connect(x =>
				{
					x.AddConsumerOf<DebugLogMessage>()
						.Where(m => m.Level.IsEnabledForLevel(logLevel))
						.HandleOnCallingThread()
						.OnCurrentSynchronizationContext()
						.UsingConsumer(m => WriteLine(m, ConsoleColor.Cyan))
						.HandleOnFiber(consoleFiber);
					x.AddConsumerOf<InfoLogMessage>()
						.Where(m => m.Level.IsEnabledForLevel(logLevel))
						.HandleOnCallingThread()
						.OnCurrentSynchronizationContext()
						.UsingConsumer(m => WriteLine(m, ConsoleColor.White))
						.HandleOnFiber(consoleFiber);
					x.AddConsumerOf<WarnLogMessage>()
						.Where(m => m.Level.IsEnabledForLevel(logLevel))
						.HandleOnCallingThread()
						.OnCurrentSynchronizationContext()
						.UsingConsumer(m => WriteLine(m, ConsoleColor.Yellow))
						.HandleOnFiber(consoleFiber);
					x.AddConsumerOf<ErrorLogMessage>()
						.Where(m => m.Level.IsEnabledForLevel(logLevel))
						.HandleOnCallingThread()
						.OnCurrentSynchronizationContext()
						.UsingConsumer(m => WriteLine(m, ConsoleColor.Red))
						.HandleOnFiber(consoleFiber);
					x.AddConsumerOf<FatalLogMessage>()
						.Where(m => m.Level.IsEnabledForLevel(logLevel))
						.HandleOnCallingThread()
						.OnCurrentSynchronizationContext()
						.UsingConsumer(m => WriteLine(m, ConsoleColor.Red))
						.HandleOnFiber(consoleFiber);
				});
		}
	}
}
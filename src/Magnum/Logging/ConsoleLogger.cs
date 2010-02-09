// Copyright 2007-2008 The Apache Software Foundation.
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

	public class ConsoleLogger :
		LoggerFacade
	{
		public ConsoleLogger(string name, ConsoleLogProvider provider)
			: base(name, GetDebug(provider), GetInfo(provider), GetWarn(provider), GetError(provider), GetFatal(provider))
		{
		}

		private static ILogWriter GetDebug(ConsoleLogProvider log)
		{
			return new ConsoleLogWriter(() => log.IsEnabled(LogLevel.Debug), ConsoleColor.Cyan);
		}

		private static ILogWriter GetInfo(ConsoleLogProvider log)
		{
			return new ConsoleLogWriter(() => log.IsEnabled(LogLevel.Info), ConsoleColor.White);
		}

		private static ILogWriter GetWarn(ConsoleLogProvider log)
		{
			return new ConsoleLogWriter(() => log.IsEnabled(LogLevel.Warn), ConsoleColor.Yellow);
		}

		private static ILogWriter GetError(ConsoleLogProvider log)
		{
			return new ConsoleLogWriter(() => log.IsEnabled(LogLevel.Error), ConsoleColor.Red);
		}

		private static ILogWriter GetFatal(ConsoleLogProvider log)
		{
			return new ConsoleLogWriter(() => log.IsEnabled(LogLevel.Fatal), ConsoleColor.Red);
		}
	}
}
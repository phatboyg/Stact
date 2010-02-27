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

	public static class Logger
	{
		private static readonly object _changeProviderLock = new object();

		private static ILogProvider _defaultTraceLogger;
		private static Func<ILogProvider> _getLogProvider = GetDefaultTraceLogger;

		public static Func<ILogProvider> SetLogProvider(Func<ILogProvider> getProvider)
		{
			lock (_changeProviderLock)
			{
				Func<ILogProvider> previousProvider = _getLogProvider;

				_getLogProvider = getProvider;

				return previousProvider;
			}
		}

		public static ILogger GetLogger<T>()
		{
			return _getLogProvider().GetLogger<T>();
		}

		public static ILogger GetLogger(string name)
		{
			return _getLogProvider().GetLogger(name);
		}

		private static ILogProvider GetDefaultTraceLogger()
		{
			if (_defaultTraceLogger != null)
				return _defaultTraceLogger;

			lock (_changeProviderLock)
			{
				if (_defaultTraceLogger != null)
					return _defaultTraceLogger;

				_defaultTraceLogger = new TraceLogProvider(LogLevel.Error);
			}

			return _defaultTraceLogger;
		}
	}
}
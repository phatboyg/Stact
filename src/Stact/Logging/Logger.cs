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
	using Channels;
	using Collections;
	using Internal;


	public static class Logger
	{
		static readonly UntypedChannel _logChannel = new ChannelAdapter();
		static readonly Cache<string, ILogger> _loggers = new Cache<string, ILogger>(CreateLogger);

		public static UntypedChannel LogChannel
		{
			get { return _logChannel; }
		}

		public static ILogger GetLogger<T>()
		{
			return GetLogger(typeof(T).FullName);
		}

		public static ILogger GetLogger(string name)
		{
			return _loggers[name];
		}

		static ILogger CreateLogger(string source)
		{
			return new LogMessageLogger(source, _logChannel);
		}
	}
}
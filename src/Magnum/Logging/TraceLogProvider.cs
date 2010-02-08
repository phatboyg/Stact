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

	public class TraceLogProvider :
		ILogProvider
	{
		public TraceLogProvider()
		{
			Level = LogLevel.Error;
		}

		public LogLevel Level { get; private set; }

		public ILogger GetLogger<T>()
		{
			return GetLogger(typeof (T).FullName);
		}

		public ILogger GetLogger(string name)
		{
			return new TraceLogger(name, this);
		}

		public void SetLogLevel(LogLevel level)
		{
			Level = level;
		}

		public void SetLogLevel(string name, LogLevel level)
		{
			throw new NotImplementedException("This would be nice, but not mandatory at this point");
		}

		public bool IsEnabled(LogLevel level)
		{
			return (int) level >= (int) Level;
		}
	}
}
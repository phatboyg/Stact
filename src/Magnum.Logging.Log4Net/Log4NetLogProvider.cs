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
namespace Magnum.Logging.Log4Net
{
	using System;
	using log4net;

	public class Log4NetLogProvider :
		ILogProvider
	{
		public ILogger GetLogger<T>()
		{
			return GetLogger(typeof (T).FullName);
		}

		public ILogger GetLogger(string name)
		{
			ILog logger = LogManager.GetLogger(name);

			return new Log4NetLogger(name, logger);
		}

		public static void Configure()
		{
			try
			{
				LogManager.GetLogger(typeof (Log4NetLogProvider));
			}
			catch (Exception ex)
			{
				throw new LoggerException("The Log4Net assembly is not referenced or could not be initialized", ex);
			}
		}
	}
}
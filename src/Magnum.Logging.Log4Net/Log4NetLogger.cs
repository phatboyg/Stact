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
	using log4net;

	public class Log4NetLogger :
		LoggerFacade
	{
		readonly ILog _log;

		public Log4NetLogger(string name, ILog log)
			: base(name, GetDebug(log), GetInfo(log), GetWarn(log), GetError(log), GetFatal(log))
		{
			_log = log;
		}

		static ILogWriter GetDebug(ILog log)
		{
			return new Log4NetLogWriter(() => log.IsDebugEnabled, o => log.Debug(o), (x, o) => log.Debug(o, x));
		}

		static ILogWriter GetInfo(ILog log)
		{
			return new Log4NetLogWriter(() => log.IsInfoEnabled, o => log.Info(o), (x, o) => log.Info(o, x));
		}

		static ILogWriter GetWarn(ILog log)
		{
			return new Log4NetLogWriter(() => log.IsWarnEnabled, o => log.Warn(o), (x, o) => log.Warn(o, x));
		}

		static ILogWriter GetError(ILog log)
		{
			return new Log4NetLogWriter(() => log.IsErrorEnabled, o => log.Error(o), (x, o) => log.Error(o, x));
		}

		static ILogWriter GetFatal(ILog log)
		{
			return new Log4NetLogWriter(() => log.IsFatalEnabled, o => log.Fatal(o), (x, o) => log.Fatal(o, x));
		}
	}
}
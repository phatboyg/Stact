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
namespace Magnum.ForLog4Net
{
	using Channels;
	using Fibers;
	using log4net;
	using Logging.Messages;


	public class Log4NetLogInstance
	{
		readonly Fiber _fiber;
		readonly ILog _log;

		public Log4NetLogInstance(Fiber fiber, string source)
		{
			_fiber = fiber;
			_log = LogManager.GetLogger(source);

			DebugChannel = new ConsumerChannel<DebugLogMessage>(_fiber, Debug);
			ErrorChannel = new ConsumerChannel<ErrorLogMessage>(_fiber, Error);
			FatalChannel = new ConsumerChannel<FatalLogMessage>(_fiber, Fatal);
			InfoChannel = new ConsumerChannel<InfoLogMessage>(_fiber, Info);
			WarnChannel = new ConsumerChannel<WarnLogMessage>(_fiber, Warn);
		}

		public Channel<DebugLogMessage> DebugChannel { get; private set; }
		public Channel<ErrorLogMessage> ErrorChannel { get; private set; }
		public Channel<FatalLogMessage> FatalChannel { get; private set; }
		public Channel<InfoLogMessage> InfoChannel { get; private set; }
		public Channel<WarnLogMessage> WarnChannel { get; private set; }

		void Debug(DebugLogMessage message)
		{
			if (!_log.IsDebugEnabled)
				return;

			if (message.Exception != null)
				_log.Debug(message.Message + (message.Exception));
			else
				_log.Debug(message.Message);
		}

		void Error(ErrorLogMessage message)
		{
			if (!_log.IsErrorEnabled)
				return;

			if (message.Exception != null)
				_log.Error(message.Message + (message.Exception));
			else
				_log.Error(message.Message);
		}

		void Fatal(FatalLogMessage message)
		{
			if (!_log.IsFatalEnabled)
				return;

			if (message.Exception != null)
				_log.Fatal(message.Message + (message.Exception));
			else
				_log.Fatal(message.Message);
		}

		void Info(InfoLogMessage message)
		{
			if (!_log.IsInfoEnabled)
				return;

			if (message.Exception != null)
				_log.Info(message.Message + (message.Exception));
			else
				_log.Info(message.Message);
		}

		void Warn(WarnLogMessage message)
		{
			if (!_log.IsWarnEnabled)
				return;

			if (message.Exception != null)
				_log.Warn(message.Message + (message.Exception));
			else
				_log.Warn(message.Message);
		}
	}
}
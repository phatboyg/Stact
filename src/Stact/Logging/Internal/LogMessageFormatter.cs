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
namespace Stact.Logging.Internal
{
	using System;
	using Channels;
	using Extensions;
	using Messages;


	public class LogMessageFormatter<T> :
		LogWriter
		where T : LogMessage
	{
		const string NullString = "null";
		readonly Func<Func<string>, Exception, T> _messageFactory;
		readonly UntypedChannel _output;

		public LogMessageFormatter(UntypedChannel output, Func<Func<string>, Exception, T> messageFactory)
		{
			_output = output;
			_messageFactory = messageFactory;
		}

		public void Write(string format, params object[] args)
		{
			Send(() => string.Format(format, args));
		}

		public void Write(IFormatProvider provider, string format, params object[] args)
		{
			Send(() => string.Format(provider, format, args));
		}

		public void Write(Exception exception, string format, params object[] args)
		{
			Send(() => string.Format(format, args), exception);
		}

		public void Write(Exception exception, IFormatProvider provider, string format, params object[] args)
		{
			Send(() => string.Format(provider, format, args), exception);
		}

		public void Write(Action<LogWriter> action)
		{
			action(this);
		}

		public void Write(object obj)
		{
			Send(() => obj != null ? obj.ToString() : NullString);
		}

		public void Write(string message)
		{
			if (message.IsNotEmpty())
				Send(() => message);
		}

		public void Write(Exception exception)
		{
			Send(() => "", exception);
		}

		public void Write(Exception exception, string message)
		{
			Send(() => message, exception);
		}

		void Send(Func<string> generator)
		{
			T message = _messageFactory(generator, null);

			_output.Send(message);
		}

		void Send(Func<string> generator, Exception exception)
		{
			T message = _messageFactory(generator, exception);

			_output.Send(message);
		}
	}
}
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

	public class Log4NetLogWriter :
		ILogWriter
	{
		readonly Func<bool> _enabled;
		readonly Action<Exception, object> _exceptionAction;
		readonly Action<object> _logAction;

		public Log4NetLogWriter(Func<bool> enabled, Action<object> logAction, Action<Exception, object> exceptionAction)
		{
			_enabled = enabled;
			_logAction = logAction;
			_exceptionAction = exceptionAction;
		}

		public void Write(object obj)
		{
			if (_enabled())
				_logAction(obj);
		}

		public void Write(string message)
		{
			if (_enabled())
				_logAction(message);
		}

		public void Write(string format, params object[] args)
		{
			if (_enabled())
				_logAction(string.Format(format, args));
		}

		public void Write(IFormatProvider provider, string format, params object[] args)
		{
			if (_enabled())
				_logAction(string.Format(provider, format, args));
		}

		public void Write(Exception exception)
		{
			if (_enabled())
				_exceptionAction(exception, string.Empty);
		}

		public void Write(Exception exception, string message)
		{
			if (_enabled())
				_exceptionAction(exception, message);
		}

		public void Write(Exception exception, string format, params object[] args)
		{
			if (_enabled())
				_exceptionAction(exception, string.Format(format, args));
		}

		public void Write(Exception exception, IFormatProvider provider, string format, params object[] args)
		{
			if (_enabled())
				_exceptionAction(exception, string.Format(provider, format, args));
		}

		public void Write(Action<ILogWriter> action)
		{
			if (_enabled())
				action(this);
		}
	}
}
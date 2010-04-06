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
	using System.Diagnostics;
	using System.Threading;
	using Extensions;

	public class TraceLogWriter :
		ILogWriter
	{
		const string NullString = "null";
		readonly Func<bool> _enabled;

		public TraceLogWriter(Func<bool> enabled)
		{
			_enabled = enabled;
		}

		public void Write(string format, params object[] args)
		{
			if (_enabled())
				WriteLine(string.Format(format, args));
		}

		public void Write(IFormatProvider provider, string format, params object[] args)
		{
			if (_enabled())
				WriteLine(string.Format(provider, format, args));
		}

		public void Write(Exception exception, string format, params object[] args)
		{
			if (_enabled())
			{
				WriteLine(string.Format(format, args));
				Trace.WriteLine(exception);
			}
		}

		public void Write(Exception exception, IFormatProvider provider, string format, params object[] args)
		{
			if (_enabled())
			{
				WriteLine(string.Format(provider, format, args));
				Trace.WriteLine(exception);
			}
		}

		public void Write(Action<ILogWriter> action)
		{
			if (_enabled())
				action(this);
		}

		public void Write(object obj)
		{
			if (_enabled())
				WriteLine(obj != null ? obj.ToString() : NullString);
		}

		public void Write(string message)
		{
			if (_enabled() && message.IsNotEmpty())
				WriteLine(message);
		}

		public void Write(Exception exception)
		{
			if (_enabled())
				WriteLine(exception != null ? exception.ToString() : NullString);
		}

		public void Write(Exception exception, string message)
		{
			if (_enabled())
			{
				WriteLine(message);
				Trace.WriteLine(exception);
			}
		}

		private void WriteLine(string text)
		{
			DateTime now = SystemUtil.Now;

			string date = now.ToShortDateString();
			string time = now.ToLongTimeString();
			string thread = Thread.CurrentThread.ManagedThreadId.ToString();

			Trace.WriteLine(string.Format("{0} {1} [{2}]: {3}", date, time, thread, text));
		}
	}
}
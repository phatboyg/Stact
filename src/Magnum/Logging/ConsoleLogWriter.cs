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
	using ObjectExtensions;

	public class ConsoleLogWriter :
		ILogWriter
	{
		private const string NullString = "null";
		private readonly Func<bool> _enabled;
		private readonly ConsoleColor _color;

		public ConsoleLogWriter(Func<bool> enabled, ConsoleColor color)
		{
			_enabled = enabled;
			_color = color;
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
				WriteLine(exception.ToString());
			}
		}

		public void Write(Exception exception, IFormatProvider provider, string format, params object[] args)
		{
			if (_enabled())
			{
				WriteLine(string.Format(provider, format, args));
				WriteLine(exception.ToString());
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
			if (_enabled() && !message.IsNullOrEmpty())
				WriteLine(message);
		}

		public void Write(Exception exception)
		{
			if (_enabled())
				WriteLine(exception.ToString());
		}

		public void Write(Exception exception, string message)
		{
			if (_enabled())
			{
				WriteLine(message);
				WriteLine(exception.ToString());
			}
		}

		private void WriteLine(string line)
		{
			if(_color == ConsoleColor.White)
			{
				Console.WriteLine(line);
				return;
			}

			Console.ForegroundColor = _color;
			Console.WriteLine(line);
			Console.ResetColor();
		}
	}
}
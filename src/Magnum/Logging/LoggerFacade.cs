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

	/// <summary>
	/// Abstract base class which provides most logging method
	/// overloads for an <see cref="ILogger"/> implementation.
	/// </summary>
	public abstract class LoggerFacade :
		ILogger
	{
		readonly ILogWriter _debug;
		readonly ILogWriter _error;
		readonly ILogWriter _fatal;
		readonly ILogWriter _info;
		readonly ILogWriter _warn;

		protected LoggerFacade(string name, ILogWriter debug, ILogWriter info, ILogWriter warn, ILogWriter error, ILogWriter fatal)
		{
			Name = name;

			_debug = debug;
			_info = info;
			_warn = warn;
			_error = error;
			_fatal = fatal;
		}

		public string Name { get; private set; }

		public void Debug(object obj)
		{
			_debug.Write(obj);
		}

		public void Debug(string message)
		{
			_debug.Write(message);
		}

		public void Debug(Exception exception)
		{
			_debug.Write(exception);
		}

		public void Debug(Exception exception, string message)
		{
			_debug.Write(exception, message);
		}

		public void Debug(Action<ILogWriter> logAction)
		{
			_debug.Write(logAction);
		}

		public void Info(object obj)
		{
			_info.Write(obj);
		}

		public void Info(string message)
		{
			_info.Write(message);
		}

		public void Info(Exception exception)
		{
			_info.Write(exception);
		}

		public void Info(Exception exception, string message)
		{
			_info.Write(exception, message);
		}

		public void Info(Action<ILogWriter> logAction)
		{
			_info.Write(logAction);
		}

		public void Warn(object obj)
		{
			_warn.Write(obj);
		}

		public void Warn(string message)
		{
			_warn.Write(message);
		}

		public void Warn(Exception exception)
		{
			_warn.Write(exception);
		}

		public void Warn(Exception exception, string message)
		{
			_warn.Write(exception, message);
		}

		public void Warn(Action<ILogWriter> logAction)
		{
			_warn.Write(logAction);
		}

		public void Error(object obj)
		{
			_error.Write(obj);
		}

		public void Error(string message)
		{
			_error.Write(message);
		}

		public void Error(Exception exception)
		{
			_error.Write(exception);
		}

		public void Error(Exception exception, string message)
		{
			_error.Write(exception, message);
		}

		public void Error(Action<ILogWriter> logAction)
		{
			_error.Write(logAction);
		}

		public void Fatal(object obj)
		{
			_fatal.Write(obj);
		}

		public void Fatal(string message)
		{
			_fatal.Write(message);
		}

		public void Fatal(Exception exception)
		{
			_fatal.Write(exception);
		}

		public void Fatal(Exception exception, string message)
		{
			_fatal.Write(exception, message);
		}

		public void Fatal(Action<ILogWriter> logAction)
		{
			_fatal.Write(logAction);
		}
	}
}
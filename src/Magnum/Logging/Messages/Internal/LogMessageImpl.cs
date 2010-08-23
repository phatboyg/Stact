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
namespace Magnum.Logging.Messages.Internal
{
	using System;
	using System.Collections.Generic;
	using Extensions;


	public abstract class LogMessageImpl
	{
		readonly string _domainName;
		readonly string _exception;
		readonly LogLevel _level;
		readonly Func<string> _messageBuilder;
		readonly string _source;
		readonly string _threadName;
		readonly DateTime _timestamp;
		readonly string _userName;
		readonly IDictionary<string, string> _properties;
		string _message;

		protected LogMessageImpl(string source, LogLevel level, Func<string> messageBuilder, Exception exception)
			: this(source, level, messageBuilder)
		{
			if(exception != null)
				_exception = exception.ToString();
		}

		protected LogMessageImpl(string source, LogLevel level, Func<string> messageBuilder)
		{
			_timestamp = SystemUtil.UtcNow;
			_domainName = AppDomain.CurrentDomain.FriendlyName;
			_threadName = System.Threading.Thread.CurrentThread.Name
			          ?? "ThreadId-{0}".FormatWith(System.Threading.Thread.CurrentThread.ManagedThreadId);
			_userName = GetCurrentUserName();
			_source = source;

			_properties = new Dictionary<string, string>();
			_messageBuilder = messageBuilder;
			_level = level;
		}

		public IDictionary<string, string> Properties
		{
			get { return _properties; }
		}

		public DateTime Timestamp
		{
			get { return _timestamp; }
		}

		public string DomainName
		{
			get { return _domainName; }
		}

		public string ThreadName
		{
			get { return _threadName; }
		}

		public string UserName
		{
			get { return _userName; }
		}

		public LogLevel Level
		{
			get { return _level; }
		}

		public string Source
		{
			get { return _source; }
		}

		public string Message
		{
			get { return _message ?? (_message = _messageBuilder()); }
		}

		public string Exception
		{
			get { return _exception; }
		}

		string GetCurrentUserName()
		{
			if (System.Threading.Thread.CurrentPrincipal == null)
				return null;

			if (System.Threading.Thread.CurrentPrincipal.Identity == null)
				return System.Threading.Thread.CurrentPrincipal.ToString();

			return System.Threading.Thread.CurrentPrincipal.Identity.Name;
		}
	}
}
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

	public class LogWriter :
		ILogWriter,
		ILogSource
	{
		const string NullString = "null";

		public LogWriter(ILogProvider provider, string name, LogLevel level)
		{
			Provider = provider;
			Name = name;
			Level = level;
		}

		public ILogProvider Provider { get; private set; }
		public string Name { get; private set; }
		public LogLevel Level { get; private set; }

		public void Write(string format, params object[] args)
		{
			if (Provider.IsLogSourceEnabled(this))
			{
				Provider.Log(this, format, args);
			}
		}

		public void Write(IFormatProvider provider, string format, params object[] args)
		{
			if (Provider.IsLogSourceEnabled(this))
			{
				Provider.Log(this, provider, format, args);
			}
		}

		public void Write(Exception exception, string format, params object[] args)
		{
			if (Provider.IsLogSourceEnabled(this))
			{
				Provider.Log(this, exception, format, args);
			}
		}

		public void Write(Exception exception, IFormatProvider provider, string format, params object[] args)
		{
			if (Provider.IsLogSourceEnabled(this))
			{
				Provider.Log(this, exception, provider, format, args);
			}
		}

		public void Write(object obj)
		{
			if (Provider.IsLogSourceEnabled(this))
			{
				Provider.Log(this, obj != null ? obj.ToString() : NullString);
			}
		}

		public void Write(string message)
		{
			if (Provider.IsLogSourceEnabled(this))
			{
				Provider.Log(this, message);
			}
		}

		public void Write(Exception exception)
		{
			if (Provider.IsLogSourceEnabled(this))
			{
				Provider.Log(this, exception);
			}
		}

		public void Write(Exception exception, string message)
		{
			if (Provider.IsLogSourceEnabled(this))
			{
				Provider.Log(this, exception, message);
			}
		}
	}
}
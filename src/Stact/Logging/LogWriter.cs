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
namespace Stact.Logging
{
	using System;

	/// <summary>
	/// These are only provided in the action syntax since it can be inefficient to generate
	/// the format string if the log level is not set to a level that will allow the output
	/// to be generated.
	/// </summary>
	public interface LogWriter
	{
		void Write(object obj);
		void Write(string message);
		void Write(string format, params object[] args);
		void Write(IFormatProvider provider, string format, params object[] args);
		void Write(Exception exception);
		void Write(Exception exception, string message);
		void Write(Exception exception, string format, params object[] args);
		void Write(Exception exception, IFormatProvider provider, string format, params object[] args);
		void Write(Action<LogWriter> logAction);
	}
}
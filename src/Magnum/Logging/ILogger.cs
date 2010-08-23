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

	public interface ILogger
	{
		void Debug(object obj);
		void Debug(string message);
		void Debug(Exception exception);
		void Debug(Exception exception, string message);
		void Debug(Action<LogWriter> logAction);

		void Info(object obj);
		void Info(string message);
		void Info(Exception exception);
		void Info(Exception exception, string message);
		void Info(Action<LogWriter> logAction);

		void Warn(object obj);
		void Warn(string message);
		void Warn(Exception exception);
		void Warn(Exception exception, string message);
		void Warn(Action<LogWriter> logAction);

		void Error(object obj);
		void Error(string message);
		void Error(Exception exception);
		void Error(Exception exception, string message);
		void Error(Action<LogWriter> logAction);

		void Fatal(object obj);
		void Fatal(string message);
		void Fatal(Exception exception);
		void Fatal(Exception exception, string message);
		void Fatal(Action<LogWriter> logAction);
	}
}
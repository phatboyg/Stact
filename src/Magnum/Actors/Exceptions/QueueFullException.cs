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
namespace Magnum.Actors.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	public class QueueFullException : Exception
	{
		private readonly int _queueDepth;

		public QueueFullException()
		{
		}

		public QueueFullException(int queueDepth)
			: this("The queue is full and cannot accept new commands (" + queueDepth + ")")
		{
			_queueDepth = queueDepth;
		}

		public QueueFullException(string message)
			:
				base(message)
		{
		}

		public QueueFullException(string message, Exception innerException)
			:
				base(message, innerException)
		{
		}

		protected QueueFullException(SerializationInfo info, StreamingContext context)
			:
				base(info, context)
		{
		}

		public int Depth
		{
			get { return _queueDepth; }
		}
	}
}
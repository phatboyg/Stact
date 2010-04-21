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
namespace Magnum.Fibers
{
	using System;
	using System.Runtime.Serialization;

	public class FiberOverrunException :
		FiberException
	{
		public FiberOverrunException()
		{
		}

		public FiberOverrunException(int needed, int count, int limit)
			: base(string.Format("Insufficient space in queue, requested {0}, currently {1} of {2} used", needed, count, limit))
		{
		}

		public FiberOverrunException(string message)
			: base(message)
		{
		}

		public FiberOverrunException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected FiberOverrunException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
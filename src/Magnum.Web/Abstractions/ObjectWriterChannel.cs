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
namespace Magnum.Web.Abstractions
{
	using System;
	using Actions;
	using Channels;

	/// <summary>
	/// Provides a channel that can be used to write an object out to the ObjectWriter,
	/// which will convert it to the appropriate type (json, xml, etc.)
	/// </summary>
	/// <typeparam name="T">The channel type</typeparam>
	public class ObjectWriterChannel<T> :
		Channel<T>
	{
		private readonly Action<Channel<T>> _completed;
		private readonly ActionQueue _queue;
		private readonly ObjectWriter _writer;

		public ObjectWriterChannel(ActionQueue queue, ObjectWriter writer, Action<Channel<T>> completed)
		{
			_writer = writer;
			_queue = queue;
			_completed = completed;
		}

		/// <summary>
		/// Enqueues the message to be written, after which it will invoke the completed action
		/// to complete the request/response exchange
		/// </summary>
		/// <param name="message"></param>
		public void Send(T message)
		{
			_queue.Enqueue(() =>
				{
					_writer.Write(message);

					_completed(this);
				});
		}
	}
}
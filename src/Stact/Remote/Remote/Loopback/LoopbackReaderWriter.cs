// Copyright 2010 Chris Patterson
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
namespace Stact.Remote.Loopback
{
	using System;
	using Magnum.Extensions;


	public class LoopbackReaderWriter :
		ChunkWriter,
		IDisposable
	{
		readonly Fiber _fiber;
		bool _disposed;
		Channel<ArraySegment<byte>> _output;

		public LoopbackReaderWriter(Fiber fiber)
		{
			_fiber = fiber;
		}

		public Channel<ArraySegment<byte>> Output
		{
			get { return _output; }
			set { _output = value; }
		}

		public void Write(ArraySegment<byte> chunk, Action<ArraySegment<byte>> unsentCallback)
		{
			_fiber.Add(() =>
			{
				if (_output != null)
					_output.Send(chunk);
			});
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~LoopbackReaderWriter()
		{
			Dispose(false);
		}

		void Dispose(bool disposing)
		{
			if (_disposed)
				return;
			if (disposing)
				_fiber.Stop(1.Minutes());

			_disposed = true;
		}
	}
}
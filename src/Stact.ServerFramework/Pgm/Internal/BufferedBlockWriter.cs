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
namespace Stact.ServerFramework.Internal
{
	using System;
	using System.Diagnostics;
	using System.Threading;
	using Magnum;
	using Magnum.Extensions;


	public class BufferedBlockWriter :
		BlockWriter,
		IDisposable
	{
		readonly Fiber _fiber;
		readonly object _lock = new object();
		readonly BlockWriter _output;
		readonly Scheduler _scheduler;
		long _blockCount;
		long _byteCount;
		ByteBuffer _inputBuffer;
		long _messageCount;
		int _minFlushLength;
		ByteBuffer _outputBuffer;
		Stopwatch _readInterval;

		bool _running = true;

		TimeSpan _waitAfterMinFlush;

		public BufferedBlockWriter(Fiber fiber, Scheduler scheduler, BlockWriter output, int bufferLengthInBytes)
		{
			_waitAfterMinFlush = 10.Milliseconds();
			_fiber = fiber;
			_scheduler = scheduler;
			_output = output;

			_inputBuffer = new ByteBuffer(bufferLengthInBytes);
			_outputBuffer = new ByteBuffer(bufferLengthInBytes);

			_minFlushLength = bufferLengthInBytes/2;
		}


		public TimeSpan WaitAfterMinFlush
		{
			get { return _waitAfterMinFlush; }
			set
			{
				Guard.GreaterThan(TimeSpan.Zero, value);

				_waitAfterMinFlush = value;
			}
		}

		public int MinFlushLength
		{
			get { return _minFlushLength; }
			set
			{
				Guard.GreaterThan(0, value);
				Guard.LessThan(_inputBuffer.Capacity, value);

				_minFlushLength = value;
			}
		}

		public void Write(ArraySegment<byte> block, Action<ArraySegment<byte>> unsentCallback)
		{
			lock (_lock)
			{
				while (_running && (_inputBuffer.Length + block.Count) > _inputBuffer.Capacity)
					Monitor.Wait(_lock);

				if (!_running)
				{
					unsentCallback(block);
					return;
				}

				_inputBuffer.Write(block, unsentCallback);
				_messageCount++;

				Monitor.PulseAll(_lock);
			}
		}

		public void Start()
		{
			_fiber.Add(FlushBuffer);

			StartMetrics();
		}

		public void Dispose()
		{
			lock (_lock)
			{
				_running = false;
				Monitor.PulseAll(_lock);
			}
		}

		public BlockWriterMetrics GetMetrics()
		{
			_readInterval.Stop();

			int averageBlockSize = _blockCount > 0 ? (int)(_byteCount/_blockCount) : 0;
			long duration = _readInterval.ElapsedMilliseconds;

			var metrics = new BlockWriterMetricsImpl(duration, _messageCount, _blockCount, _byteCount, averageBlockSize);

			_readInterval.Reset();
			_readInterval.Start();

			_messageCount = 0;
			_blockCount = 0;
			_byteCount = 0;

			return metrics;
		}

		void FlushBuffer()
		{
			TimeSpan sleepInterval = TimeSpan.Zero;
			try
			{
				lock (_lock)
				{
					while (_running && _inputBuffer.Length == 0)
						Monitor.Wait(_lock);
					if (!_running)
						return;

					ByteBuffer swap = _outputBuffer;
					_outputBuffer = _inputBuffer;
					_inputBuffer = swap;

					Monitor.PulseAll(_lock);
				}

				ArraySegment<byte> content = _outputBuffer.Content;
				if (content.Count > 0)
				{
					_output.Write(content, _outputBuffer.NotifyUnsent);

					_outputBuffer.Reset();
					_blockCount++;
					_byteCount += content.Count;

					if (content.Count < _minFlushLength)
						sleepInterval = _waitAfterMinFlush;
				}
			}
			finally
			{
				if (_running)
				{
					if (sleepInterval != TimeSpan.Zero)
						_scheduler.Schedule(sleepInterval, _fiber, FlushBuffer);
					else
						_fiber.Add(FlushBuffer);
				}
			}
		}

		void StartMetrics()
		{
			_readInterval = Stopwatch.StartNew();
		}
	}
}
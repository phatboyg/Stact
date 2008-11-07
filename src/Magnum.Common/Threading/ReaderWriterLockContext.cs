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
namespace Magnum.Common.Threading
{
	using System;
	using System.Threading;

	public class ReaderWriterLockContext :
		IDisposable
	{
		private volatile bool _disposed;
		private ReaderWriterLockSlim _lock;

		public ReaderWriterLockContext()
		{
			_lock = new ReaderWriterLockSlim();
		}

		public ReaderWriterLockContext(LockRecursionPolicy policy)
		{
			_lock = new ReaderWriterLockSlim(policy);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void ReadUnlocked(Action<ReaderWriterLockContext> action)
		{
			action(this);
		}

		public void ReadLock(Action<ReaderWriterLockContext> action)
		{
			if (_disposed) throw new ObjectDisposedException("LambdaReadWriteLock");

			_lock.EnterReadLock();
			try
			{
				action(this);
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}

		public bool ReadLock(TimeSpan span, Action<ReaderWriterLockContext> action)
		{
			if (_disposed) throw new ObjectDisposedException("LambdaReadWriteLock");

			if (_lock.TryEnterReadLock(span) == false)
				return false;

			try
			{
				action(this);
			}
			finally
			{
				_lock.ExitReadLock();
			}

			return true;
		}

		public void WriteLock(Action<ReaderWriterLockContext> action)
		{
			if (_disposed) throw new ObjectDisposedException("LambdaReadWriteLock");

			_lock.EnterWriteLock();
			try
			{
				action(this);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public bool WriteLock(TimeSpan span, Action<ReaderWriterLockContext> action)
		{
			if (_disposed) throw new ObjectDisposedException("LambdaReadWriteLock");

			if (_lock.TryEnterWriteLock(span) == false)
				return false;

			try
			{
				action(this);
			}
			finally
			{
				_lock.ExitWriteLock();
			}

			return true;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			if (_lock != null)
				_lock.Dispose();

			_lock = null;
			_disposed = true;
		}

		~ReaderWriterLockContext()
		{
			Dispose(false);
		}
	}
}
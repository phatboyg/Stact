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
namespace Magnum.MapReduce.Readers
{
	using System;
	using System.Collections.Generic;
	using Common;
	using Types;

	public class LineRecordReader :
		IRecordReader<long, string>
	{
		private readonly IContentReader _reader;
		private bool _disposed;
		private IEnumerator<KeyValuePair<long, string>> _enumerator;
		private long _position;

		public LineRecordReader(IContentReader reader)
		{
			_reader = reader;

			_enumerator = _reader.GetEnumerator();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Close()
		{
		}

		public long Position
		{
			get { return _position; }
		}

		public float Progress
		{
			get { return 0.5f; }
		}

		public KeyValuePair<long, string> Next()
		{
			if (_enumerator.MoveNext())
			{
				_position = _enumerator.Current.Key;

				return _enumerator.Current;
			}

			return default(KeyValuePair<long, string>);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			if (_enumerator != null)
				_enumerator.Dispose();

			_enumerator = null;
			_disposed = true;
		}

		~LineRecordReader()
		{
			Dispose(false);
		}
	}
}
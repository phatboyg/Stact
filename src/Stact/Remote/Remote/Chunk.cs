// Copyright 2010-2013 Chris Patterson
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
namespace Stact.Remote
{
    using System;
    using System.Collections.Generic;


    public class Chunk :
        ChunkWriter
    {
        readonly byte[] _buffer;
        readonly IList<Action> _callbacks;
        int _length;

        public Chunk(int size)
        {
            _callbacks = new List<Action>();
            _buffer = new byte[size];
        }

        public int Length
        {
            get { return _length; }
        }

        public int Capacity
        {
            get { return _buffer.Length; }
        }

        public ArraySegment<byte> Content
        {
            get { return new ArraySegment<byte>(_buffer, 0, _length); }
        }

        public void Write(ArraySegment<byte> block, Action<ArraySegment<byte>> unsentCallback)
        {
            Array.Copy(block.Array, block.Offset, _buffer, _length, block.Count);
            _length += block.Count;
            _callbacks.Add(() => unsentCallback(block));
        }

        public void Reset()
        {
            _length = 0;
            _callbacks.Clear();
        }

        public void NotifyUnsent(ArraySegment<byte> obj)
        {
            foreach (Action callback in _callbacks)
                callback();
        }

        public override string ToString()
        {
            return Content.ToMemoryViewString();
        }
    }
}
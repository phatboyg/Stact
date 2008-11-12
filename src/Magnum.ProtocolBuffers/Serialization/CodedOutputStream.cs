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
namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using System.IO;
    using System.Text;

    public class CodedOutputStream :
        IDisposable
    {
        private volatile bool _disposed;
        private MemoryStream _stream;

        public CodedOutputStream()
        {
            _stream = new MemoryStream(1024);
        }

        public int Length
        {
            get { return (int) _stream.Length; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool WriteVarint32SignExtended(Int32 value)
        {
            return value < 0 ? WriteVarint64((UInt64) value) : WriteVarint32((UInt32) value);
        }

        private bool WriteVarint64(UInt64 value)
        {
            do
            {
                if (value < 0x80)
                {
                    _stream.WriteByte((byte) value);
                    return true;
                }

                _stream.WriteByte((byte) ((value & 0x7F) | 0x80));
                value >>= 7;
            } while (value != 0);

            return true;
        }

        private bool WriteVarint32(UInt32 value)
        {
            return WriteVarint64(value);
        }

        public bool WriteFixedInt32(int value)
        {
            _stream.WriteByte((byte) value);
            _stream.WriteByte((byte) (value >> 8));
            _stream.WriteByte((byte) (value >> 16));
            _stream.WriteByte((byte) (value >> 24));

            return true;
        }

        public bool WriteFixedInt64(long value)
        {
            _stream.WriteByte((byte) value);
            _stream.WriteByte((byte) (value >> 8));
            _stream.WriteByte((byte) (value >> 16));
            _stream.WriteByte((byte) (value >> 24));
            _stream.WriteByte((byte) (value >> 32));
            _stream.WriteByte((byte) (value >> 40));
            _stream.WriteByte((byte) (value >> 48));
            _stream.WriteByte((byte) (value >> 56));

            return true;
        }

        public bool WriteTag(int fieldNumber, WireType type)
        {
            return WriteTag(WireFormat.MakeTag(fieldNumber, type));
        }

        private bool WriteTag(UInt32 value)
        {
            // TODO why not just write Varint32 ??

            if (value < (1 << 7))
            {
                _stream.WriteByte((byte) value);
                return true;
            }

            if (value < (1 << 14))
            {
                _stream.WriteByte((byte) (value | 0x80));
                _stream.WriteByte((byte) (value >> 7));
                return true;
            }

            return WriteVarint32Fallback(value);
        }

        private bool WriteVarint32Fallback(UInt32 value)
        {
            return WriteVarint64(value);
        }

        public bool WriteInt32(int fieldNumber, Int32 value)
        {
            return WriteTag(fieldNumber, WireType.Varint) &&
                   WriteVarint32SignExtended(value);
        }

        public byte[] GetBytes()
        {
            return _stream.ToArray();
        }

        public void WriteString(int fieldNumber, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);

            WriteTag(fieldNumber, WireType.LengthDelimited);
            WriteVarint32((UInt32) bytes.Length);

            _stream.Write(bytes, 0, bytes.Length);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) return;

            if (_stream != null)
                _stream.Dispose();

            _stream = null;
            _disposed = true;
        }

        ~CodedOutputStream()
        {
            Dispose(false);
        }

        public bool WriteVarint(int fieldNumber, UInt64 value)
        {
            return WriteTag(fieldNumber, WireType.Varint) &&
                   WriteVarint64(value);
        }
    }
}
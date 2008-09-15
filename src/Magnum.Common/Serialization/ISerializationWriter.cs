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
namespace Magnum.Common.Serialization
{
	using System;

	public interface ISerializationWriter
	{
		void Write(bool value);
		void Write(DateTime value);
		void Write(decimal value);
		void Write(double value);
		void Write(int value);
		void Write(long value);
		void Write(string value);
		void Write(float value);
		void Write(short value);
		void Write(ushort value);
		void Write(ulong value);
		void Write(uint value);
	}
}
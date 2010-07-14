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
namespace Magnum.FileSystem.Zip
{
    using System;
    using System.Text;

    public class ZippedFile :
		File
	{
		readonly byte[] _data;
        bool _exists;

        public ZippedFile(FileName name, byte[] data, bool exists) : this(name, data)
        {
            _exists = false;    
        }
		public ZippedFile(FileName name, byte[] data)
		{
			_data = data;
			Name = name;
            _exists = true;
		}
        

		public FileName Name { get; set; }

		public bool Exists()
		{
			return _exists;
		}

		public string ReadToEnd()
		{
			return Encoding.UTF8.GetString(_data);
		}

		public void WithStream(Action<System.IO.Stream> action)
		{
			using (var stream = new System.IO.MemoryStream(_data))
			{
				action(stream);
			}
		}
	}
}
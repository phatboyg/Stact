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
namespace Magnum.Extensions
{
	using System.IO;
	using System.Text;

    public static class ExtensionsToStream
	{
		public static byte[] ReadToEnd(this Stream stream)
		{
			using (var content = new MemoryStream())
			{
				var buffer = new byte[4096];

				int read = stream.Read(buffer, 0, 4096);
				while (read > 0)
				{
					content.Write(buffer, 0, read);

					read = stream.Read(buffer, 0, 4096);
				}

				return content.ToArray();
			}
		}

        public static string ReadToEndAsText(this Stream stream)
        {
            return Encoding.UTF8.GetString(stream.ReadToEnd());
        }
	}
}
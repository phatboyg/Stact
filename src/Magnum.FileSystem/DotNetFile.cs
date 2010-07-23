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
namespace Magnum.FileSystem
{
    using System;
    using System.IO;

    public class DotNetFile :
		File
	{
		public DotNetFile(FileName name)
		{
			Name = name;
		}

		public Directory Parent
		{
			get
			{
				var fi = new FileInfo(Name.GetPath());
				return new DotNetDirectory(DirectoryName.GetDirectoryName(fi.DirectoryName));
			}
		}

		public FileName Name { get; set; }

		public bool Exists()
		{
			return System.IO.File.Exists(Name.ToString());
		}

		public string ReadToEnd()
		{
			return System.IO.File.ReadAllText(Name.GetPath());
		}

		public void WithStream(Action<Stream> action)
		{
			using (FileStream stream = System.IO.File.OpenRead(Name.GetPath()))
			{
				action(stream);
			}
		}

        public void CopyTo(FileName path)
        {
            //refactor?
            if (System.IO.File.Exists(path.GetPath()))
                System.IO.File.Delete(path.GetPath());

            WithStream(s=>
            {
                using(var stream = System.IO.File.OpenWrite(path.GetPath()))
                {
                    //.net 4.0 has a stream.CopyTo method
                    var buff = new byte[32768];
                    while (true)
                    {
                        int read = s.Read(buff, 0, buff.Length);

                        if (read <= 0)
                            return;

                        stream.Write(buff, 0, read);
                    }
                }
            });
        }
	}
}
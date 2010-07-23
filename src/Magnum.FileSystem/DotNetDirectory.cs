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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DotNetDirectory :
		Directory
	{
		public DotNetDirectory(DirectoryName directoryName)
		{
			Name = directoryName;
		}

		public IEnumerable<File> GetFiles()
		{
			return System.IO.Directory.GetFiles(Name.GetPath())
				.Select(file => (File)new DotNetFile(FileName.GetFileName(file)));
		}

		public IEnumerable<Directory> GetDirectories()
		{
			return System.IO.Directory.GetDirectories(Name.GetPath())
				.Select(directory => (Directory)new DotNetDirectory(DirectoryName.GetDirectoryName(directory)));
		}

		public Directory GetChildDirectory(string name)
		{
			DirectoryName directoryName = Name.Combine(name);

			return new DotNetDirectory(directoryName);
		}

		public DirectoryName Name { get; private set; }

		public bool Exists()
		{
			return System.IO.Directory.Exists(Name.ToString());
		}

		public File GetChildFile(string name)
		{
			return new DotNetFile(Name.GetFileName(name));
		}

		public Directory Parent
		{
			get
			{
				var di = new DirectoryInfo(Name.GetPath());
				if (di.Parent == null)
					throw new InvalidOperationException("No parent folder: " + di.FullName);

				return new DotNetDirectory(DirectoryName.GetDirectoryName(di.Parent.FullName));
			}
		}

		public bool HasParentDir
		{
			get
			{
				var di = new DirectoryInfo(Name.GetPath());
				return di.Parent != null;
			}
		}

		public bool IsRoot()
		{
			var di = new DirectoryInfo(Name.GetPath());
			return di.Root.Name.Replace("\\", "").Equals(Name.GetPath());
		}

		public override string ToString()
		{
			return Name.GetPath();
		}

        public void CopyTo(DirectoryName path)
        {
            //refactor this out?
            if (!System.IO.Directory.Exists(path.GetPath()))
                System.IO.Directory.CreateDirectory(path.GetPath());

            // Copy all files.
            var files = GetFiles();

            foreach (var file in files)
            {
                var fileDestination = FileName.GetFileName(path, file.Name.GetName());
                file.CopyTo(fileDestination);
            }

            // Process subdirectories.
            var dirs = GetDirectories();
            foreach (var dir in dirs)
            {
                var destinationDir = DirectoryName.GetDirectoryName(path, dir.Name.GetName());
                dir.CopyTo(destinationDir);
            }
        }
	}
}
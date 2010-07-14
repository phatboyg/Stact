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
    using System.Collections.Generic;
    using System.Linq;

    public class ZippedDirectory :
		Directory
	{
		readonly IDictionary<string, ZippedDirectory> _directories = new Dictionary<string, ZippedDirectory>();
		readonly IDictionary<string, ZippedFile> _files = new Dictionary<string, ZippedFile>();
        bool _exists;

        public ZippedDirectory(DirectoryName name, Directory parent, bool exists) :
            this(name, parent)
        {
            _exists = false;
        }
		public ZippedDirectory(DirectoryName name, Directory parent)
		{
			Name = name;
			Parent = parent;
		    _exists = true;
		}

		public DirectoryName Name { get; set; }

		public Directory Parent { get; private set; }

		public bool HasParentDir
		{
			get { return true; }
		}

		public IEnumerable<File> GetFiles()
		{
			return _files.Values.Cast<File>();
		}

		public IEnumerable<Directory> GetDirectories()
		{
			return _directories.Values.Cast<Directory>();
		}

		public Directory GetChildDirectory(string name)
		{
			if (_directories.ContainsKey(name))
				return _directories[name];

			throw new InvalidOperationException("The directory '" + name + "' does not exist in '" + Name + "'");
		}

		public bool Exists()
		{
			return _exists;
		}

		public File GetChildFile(string name)
		{
			if (_files.ContainsKey(name))
				return _files[name];

			throw new InvalidOperationException("The file '" + name + "' does not exist in '" + Name + "'");
		}

		public bool IsRoot()
		{
			return false;
		}

        public void AddFile(string file, ZippedFile zipfile)
        {
            _files.Add(file, zipfile);
        }
	}
}
// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Magnum.FileSystem.Opc
{
    using System;
    using System.Collections.Generic;
    using System.IO.Packaging;
    using System.Linq;

    public class OpcFileDirectory :
        Directory
    {
        readonly IDictionary<string, OpcDirectory> _directories;
        readonly IDictionary<string, OpcFile> _files;

        public OpcFileDirectory(PathName pathName)
        {
            _directories = new Dictionary<string, OpcDirectory>();
            _files = new Dictionary<string, OpcFile>();

            Name = DirectoryName.GetDirectoryName(pathName);

            Parse();
        }


        public DirectoryName Name { get; private set; }

        public Directory Parent
        {
            get { throw new NotImplementedException(); }
        }

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

        public bool Exists()
        {
            return true;
        }

        public Directory GetChildDirectory(string name)
        {
            throw new NotImplementedException();
        }

        public File GetChildFile(string name)
        {
            throw new NotImplementedException();
        }

        public bool IsRoot()
        {
            return false;
        }

        public void CopyTo(DirectoryName path)
        {
            throw new NotImplementedException();
        }

        void Parse()
        {
            using(Package package = Package.Open(Name.GetPath()))
            {
                foreach(var part in package.GetParts())
                {
                    
                }
                package.Close();
            }
        }
    }

    public class OpcFile{}
    public class OpcDirectory{}
}
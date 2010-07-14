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
namespace Magnum.FileSystem.Zip
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Extensions;
    using Ionic.Zip;

    public class ZipFileDirectory :
        Directory
    {
        //a root directory
        readonly IDictionary<string, ZippedDirectory> _directories = new Dictionary<string, ZippedDirectory>();
        readonly IDictionary<string, ZippedFile> _files = new Dictionary<string, ZippedFile>();
        Action _parse;

        public ZipFileDirectory(PathName pathName)
        {
            Name = DirectoryName.GetDirectoryName(pathName);
            _parse = Parse;

            _parse();
        }

        public Directory Parent
        {
            get
            {
                var fi = new System.IO.FileInfo(Name.GetPath());
                return new DotNetDirectory(DirectoryName.GetDirectoryName(fi.DirectoryName));
            }
        }

        public bool HasParentDir
        {
            get { return true; }
        }

        public bool IsRoot()
        {
            return false;
        }

        public bool Exists()
        {
            return true;
        }

        public DirectoryName Name { get; private set; }

        public File GetChildFile(string name)
        {
            if (_files.ContainsKey(name))
                return _files[name];

            return new ZippedFile(FileName.GetFileName(Name, name), new byte[]{}, false);
        }

        public Directory GetChildDirectory(string name)
        {
            if (_directories.ContainsKey(name))
                return _directories[name];

            return new ZippedDirectory(DirectoryName.GetDirectoryName(Name, name), this, false);
        }

        public IEnumerable<File> GetFiles()
        {
            return _files.Values.Cast<File>();
        }

        public IEnumerable<Directory> GetDirectories()
        {
            return _directories.Values.Cast<Directory>();
        }

        void Parse()
        {
            using (var stream = System.IO.File.OpenRead(Name.GetPath()))
            {
                var input = new ZipInputStream(stream);

                ZipEntry entry;
                while ((entry = input.GetNextEntry()) != null)
                {
                    Trace.WriteLine("Reading Entry: " + entry.FileName);
                    var name = entry.FileName.Split('/');

                    if (name.Length == 1) // a file
                    {
                        byte[] data = input.ReadToEnd();
                        Trace.WriteLine("Read: " + data.Length + " bytes");
                        var fn = FileName.GetFileName(this.Name, name[0]);
                        _files.Add(name[0], new ZippedFile(fn, data));
                    }
                    else
                    {
                        var first = name.First();
                        var parent = new ZippedDirectory(DirectoryName.GetDirectoryName(this.Name, first), this);
                        _directories.Add(first, parent);
                        var rest = name.Skip(1).Take(name.Length - 2);
                        var queue = new Queue<string>(rest);

                        foreach (var item in queue)
                        {
                            var dir = new ZippedDirectory(DirectoryName.GetDirectoryName(parent.Name, item), parent);
                            parent = dir;
                        }
                        var file = name.Last();

                        byte[] data = input.ReadToEnd();
                        Trace.WriteLine("Read: " + data.Length + " bytes");
                        //neds to be added
                        var zipfile = new ZippedFile(FileName.GetFileName(parent.Name, file), data);
                        parent.AddFile(file, zipfile);
                    }
                }
            }
            _parse = () => { };
        }
    }
}
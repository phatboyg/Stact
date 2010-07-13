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
    using Magnum.Logging;

    public class DotNetFileSystem :
        FileSystem
    {
        readonly ILogger _logger = Logger.GetLogger<DotNetFileSystem>();

        public DotNetFileSystem()
        {
        }

        public bool FileExists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }

        public bool DirectoryExists(string directory)
        {
            return System.IO.Directory.Exists(directory);
        }

        public void Read(string filePath, Action<Stream> action)
        {
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                action(stream);
            }
        }

        public void WorkWithTempDir(Action<Directory> tempAction)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "magnum.fs");
            tempDir = Path.Combine(tempDir, Guid.NewGuid().ToString());
            var d = new DotNetDirectory(DirectoryName.GetDirectoryName(tempDir));
            if (!d.Exists())
            {
                CreateDirectory(d);
            }
            try
            {
                tempAction(d);
            }
            finally
            {
                Delete(d);
            }
        }

        public void Delete(Directory directory)
        {
            System.IO.Directory.Delete(directory.Name.GetPath(), true);
        }

        public string GetTempFileName()
        {
            return Path.GetTempFileName();
        }

        public String ReadToEnd(string filePath)
        {
            string contents = "";
            Read(filePath, s =>
                {
                    using (var reader = new StreamReader(s))
                    {
                        contents = reader.ReadToEnd();
                    }
                });

            return contents;
        }

        public void Write(string filePath, String contents)
        {
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (var writer = new StreamWriter(fs))
                {
                    writer.Write(contents);
                    writer.Flush();
                }
            }
        }

        public void Write(string filePath, Stream file)
        {
            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                var buff = new byte[file.Length];
                file.Read(buff, 0, buff.Length);
                fs.Write(buff,0,buff.Length);
            }
        }

        public void CreateDirectory(Directory directoryPath)
        {
            System.IO.Directory.CreateDirectory(directoryPath.Name.GetPath());
        }

        public void CreateHiddenDirectory(string directoryPath)
        {
            DirectoryInfo di = System.IO.Directory.CreateDirectory(directoryPath);
            di.Attributes |= FileAttributes.Hidden;
        }

        public void Copy(string source, string destination)
        {
            System.IO.File.Copy(source, destination);
        }

        public bool IsRooted(string path)
        {
            return Path.IsPathRooted(path);
        }

        public string Combine(string firstPath, string secondPath)
        {
            return Path.Combine(firstPath, secondPath);
        }

        public char DirectorySeparatorChar
        {
            get { return Path.DirectorySeparatorChar; }
        }

        public string[] GetDirectories(string path)
        {
            return System.IO.Directory.GetDirectories(path);
        }

        public Directory GetDirectory(string path)
        {
            if (!Path.IsPathRooted(path))
                path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), path);

            return new DotNetDirectory(DirectoryName.GetDirectoryName(path));
        }

        public void CreateFile(File file)
        {
            System.IO.File.WriteAllBytes(file.Name.GetPath(), new byte[0]);
        }

        public Directory GetCurrentDirectory()
        {
            var directory = DirectoryName.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());

            return new DotNetDirectory(directory);
        }

        public void DeleteFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);
        }
    }
}
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
    using Zip;

    /// <summary>
	/// Implementation of FileSystemLocator that resolves FileName and DirectoryName into the appropriate 
	/// classes
	/// </summary>
	public class LocalFileSystemLocator :
		FileSystemLocator
	{
		public File GetFile(FileName name)
		{
			File file = ResolveFile(name.Name);

			return file;
		}

		public Directory GetDirectory(DirectoryName name)
		{
			Directory directory = ResolveDirectory(name.Name);

			return directory;
		}

		Directory ResolveDirectory(PathName name)
		{
			Directory rootDirectory = GetRootDirectory(name);

			string relativePath = name.GetAbsolutePath().Substring(rootDirectory.Name.GetPath().Length);

			string[] names = relativePath.Split('\\', '/');

			return ResolveDirectory(rootDirectory, names);
		}

		Directory ResolveDirectory(Directory directory, IEnumerable<string> children)
		{
			if (!children.Any())
				return directory;

			string childName = children.First();

			Directory info = directory.GetDirectories()
				.Where(x => string.Compare(x.Name.GetName(), childName, true) == 0)
				.SingleOrDefault();

			if (info != null)
			{
				return ResolveDirectory(info, children.Skip(1));
			}

			File file = directory.GetFiles()
				.Where(x => string.Compare(x.Name.GetName(), childName, true) == 0)
				.SingleOrDefault();

			if (file == null)
				throw new InvalidOperationException("Could not get directory: " + childName);

			if (Path.GetExtension(file.Name.GetName()) == ".zip")
			{
				var zipFileDirectory = new ZipFileDirectory(file.Name.Name);
				return ResolveDirectory(zipFileDirectory, children.Skip(1));
			}

			throw new InvalidOperationException("Could not resolve the rest of the path: " + childName);
		}

		File ResolveFile(PathName name)
		{
			Directory rootDirectory = GetRootDirectory(name);

			string relativePath = name.GetAbsolutePath().Substring(rootDirectory.Name.GetPath().Length);

			string[] names = relativePath.Split('\\', '/');

			return ResolveFile(rootDirectory, names);
		}

		Directory GetRootDirectory(PathName name)
		{
			string root = Path.GetPathRoot(name.GetAbsolutePath());

			var di = new DirectoryInfo(root);

			return new DotNetDirectory(DirectoryName.GetDirectoryName(di.FullName));
		}

		File ResolveFile(Directory directoryInfo, IEnumerable<string> children)
		{
			if (!children.Any())
				throw new InvalidOperationException("Unable to resolve file: " + directoryInfo.Name);

			string childName = children.First();

			Directory info = directoryInfo.GetDirectories()
				.Where(x => string.Compare(x.Name.GetName(), childName, true) == 0)
				.SingleOrDefault();

			if (info != null)
			{
				return ResolveFile(info, children.Skip(1));
			}

			File file = directoryInfo.GetFiles()
				.Where(x => string.Compare(x.Name.GetName(), childName, true) == 0)
				.SingleOrDefault();

			if (file == null)
				throw new InvalidOperationException("Could not get file: " + childName);


			if (!children.Skip(1).Any())
				return file;


//			if (Path.GetExtension(file.Name.GetName()) == ".zip")
//			{
//				var zipFileDirectory = new ZipFileDirectory(file.Name.Name);
//				return ResolveFile(zipFileDirectory, children.Skip(1));
//			}

			throw new NotImplementedException();
		}
	}
}
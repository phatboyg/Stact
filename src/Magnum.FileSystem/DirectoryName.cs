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
    using Internal;

    /// <summary>
	/// A specialization of the PathName class for directories
	/// </summary>
	public abstract class DirectoryName
	{
		/// <summary>
		/// The contained PathName for the directory
		/// </summary>
		public PathName Name { get; protected set; }

		/// <summary>
		/// Combines the directory name with a child directory/file name
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public abstract DirectoryName Combine(string child);

		public abstract DirectoryName Combine(PathName child);
		public abstract DirectoryName Combine(DirectoryName child);

		public abstract string GetName();
		public abstract string GetPath();

		public FileName GetFileName(string child)
		{
			return FileName.GetFileName(Name.Combine(child));
		}

		public static DirectoryName GetDirectoryName(string name)
		{
			PathName directoryName = PathName.GetPathName(name);
			if (directoryName is RelativePathName)
				return new RelativeDirectoryName(((RelativePathName)directoryName));

			if (directoryName is AbsolutePathName)
				return new AbsoluteDirectoryName(((AbsolutePathName)directoryName));

			throw new InvalidOperationException("Unable to convert path: " + name);
		}

		public static DirectoryName GetDirectoryName(PathName pathName)
		{
			return GetDirectoryName(pathName.GetPath());
		}
	}
}
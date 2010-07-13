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
	/// A specialization of the PathName class for files
	/// </summary>
	public abstract class FileName
	{
		public PathName Name { get; protected set; }

		public abstract DirectoryName GetDirectoryName();

		public virtual string GetName()
		{
			return Name.GetName();
		}

		public virtual string GetPath()
		{
			return Name.GetPath();
		}

		public static FileName GetFileName(string name)
		{
			return GetFileName(PathName.GetPathName(name));
		}

		public static FileName GetFileName(PathName pathName)
		{
			if (pathName is RelativePathName)
				return new RelativeFileName(((RelativePathName)pathName));

			if (pathName is AbsolutePathName)
				return new AbsoluteFileName(((AbsolutePathName)pathName));

			throw new InvalidOperationException("Unable to convert path: " + pathName);
		}

		public static FileName GetFileName(DirectoryName directoryName, string child)
		{
			return GetFileName(directoryName.Name.Combine(child));
		}
	}
}
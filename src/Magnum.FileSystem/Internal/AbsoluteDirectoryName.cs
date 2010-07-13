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
namespace Magnum.FileSystem.Internal
{
    using System;

    public class AbsoluteDirectoryName :
		DirectoryName
	{
		public AbsoluteDirectoryName(AbsolutePathName name)
		{
			Name = name;
		}

		public override string ToString()
		{
			return Name.ToString();
		}

		public override DirectoryName Combine(string path)
		{
			var pathName = Name.Combine(path) as AbsolutePathName;
			if (pathName == null)
				throw new InvalidOperationException("Unable to combine " + Name + " with " + path);

			return new AbsoluteDirectoryName(pathName);
		}

		public override DirectoryName Combine(PathName child)
		{
			var pathName = Name.Combine(child) as AbsolutePathName;
			if (pathName == null)
				throw new InvalidOperationException("Unable to combine " + Name + " with " + child);

			return new AbsoluteDirectoryName(pathName);
		}

		public override DirectoryName Combine(DirectoryName child)
		{
			var pathName = Name.Combine(child.Name) as AbsolutePathName;
			if (pathName == null)
				throw new InvalidOperationException("Unable to combine " + Name + " with " + child);

			return new AbsoluteDirectoryName(pathName);
		}

		public override string GetName()
		{
			return Name.GetName();
		}

		public override string GetPath()
		{
			return Name.GetPath();
		}
	}
}
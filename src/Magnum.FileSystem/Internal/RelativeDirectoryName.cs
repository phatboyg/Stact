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

    public class RelativeDirectoryName :
		DirectoryName
	{
		public RelativeDirectoryName(RelativePathName name)
		{
			Name = name;
		}

		public override string ToString()
		{
			return Name.ToString();
		}

		public override DirectoryName Combine(string path)
		{
			return InternalCombine(Name.Combine(path));
		}

		public override DirectoryName Combine(PathName child)
		{
			return InternalCombine(Name.Combine(child));
		}

		public override DirectoryName Combine(DirectoryName child)
		{
			return InternalCombine(Name.Combine(child.Name));
		}

		public override string GetName()
		{
			return Name.GetName();
		}

		public override string GetPath()
		{
			return Name.GetPath();
		}

		static DirectoryName InternalCombine(PathName combinedPathName)
		{
			var pathName = combinedPathName as RelativePathName;
			if (pathName == null)
				throw new InvalidOperationException("Unable to combine relative paths: " + combinedPathName);

			return new RelativeDirectoryName(pathName);
		}
	}
}
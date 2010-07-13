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
    using System.IO;

    public class AbsolutePathName :
		PathName
	{
		readonly string _path;

		public AbsolutePathName(string path)
		{
			_path = path;
		}

		public override PathName Combine(string child)
		{
			if (Path.IsPathRooted(child))
				return new AbsolutePathName(child);

			return new AbsolutePathName(Path.Combine(_path, child));
		}

		public override PathName Combine(PathName child)
		{
			if (child is AbsolutePathName)
				return child;

			return new AbsolutePathName(Path.Combine(_path, child.GetPath()));
		}

		public override string GetName()
		{
			return Path.GetFileName(_path);
		}

		public override string GetPath()
		{
			return _path;
		}

		public override string GetAbsolutePath()
		{
			return _path;
		}

		public override string ToString()
		{
			return _path;
		}
	}
}
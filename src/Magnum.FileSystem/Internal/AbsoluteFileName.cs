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
    using System.IO;

    public class AbsoluteFileName :
		FileName
	{
		public AbsoluteFileName(AbsolutePathName name)
		{
			Name = name;
		}

		public override string ToString()
		{
			return Name.ToString();
		}

		public override DirectoryName GetDirectoryName()
		{
			string path = GetPath();

			if (!File.Exists(path))
				throw new InvalidOperationException("The file specified does not exist: " + path);

			string directoryPath = Path.GetDirectoryName(path);
			if (directoryPath == null)
				return new AbsoluteDirectoryName(new AbsolutePathName(path));

			return new AbsoluteDirectoryName(new AbsolutePathName(directoryPath));
		}
	}
}
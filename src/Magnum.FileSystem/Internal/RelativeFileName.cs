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

    public class RelativeFileName :
		FileName
	{
		public RelativeFileName(RelativePathName name)
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

			string directoryPath = Path.GetDirectoryName(path);
			if (directoryPath == null)
				return new RelativeDirectoryName(new RelativePathName(path));

			return new RelativeDirectoryName(new RelativePathName(directoryPath));
		}
	}
}
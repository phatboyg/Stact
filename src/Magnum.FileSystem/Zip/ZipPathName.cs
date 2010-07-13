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
namespace Magnum.FileSystem.Zip
{
    using System;
    using System.IO;
    using Internal;

    public class ZipPathName :
		PathName
	{
		readonly string _contentFilePath;
		readonly string _zipFilePath;

		public ZipPathName(string zipFilePath, string contentFilePath)
		{
			_zipFilePath = zipFilePath;
			_contentFilePath = contentFilePath;
		}

		public override PathName Combine(string child)
		{
			if (Path.IsPathRooted(child))
				throw new InvalidOperationException("Cannot combine an absolute path with a zip path");

			return new ZipPathName(_zipFilePath, Path.Combine(_contentFilePath, child));
		}

		public override PathName Combine(PathName child)
		{
			if (child is AbsolutePathName)
				throw new InvalidOperationException("Cannot combine an absolute path with a zip path");

			if (child is ZipPathName)
				throw new IOException("Cannot combine a zip file path with a zip file path");

			return new ZipPathName(_zipFilePath, Path.Combine(_contentFilePath, child.GetPath()));
		}

		public override string GetName()
		{
			return Path.GetFileName(_contentFilePath);
		}

		public override string GetPath()
		{
			return Path.Combine(_zipFilePath, _contentFilePath);
		}

		public override string GetAbsolutePath()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return GetPath();
		}
	}
}
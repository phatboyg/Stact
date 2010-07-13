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
	/// <summary>
	/// Supported the resolution of FileName and DirectoryName into the appropriate File 
	/// or Directory implementations based on the available file systems.
	/// </summary>
	public interface FileSystemLocator
	{
		/// <summary>
		/// Given a FileName, returns the File interface for the requested file
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		File GetFile(FileName name);

		/// <summary>
		/// Given a DirectoryName, returns the Directory interface for the requested directory
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		Directory GetDirectory(DirectoryName name);
	}
}
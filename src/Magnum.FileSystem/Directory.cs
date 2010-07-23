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
    using System.Collections.Generic;

    /// <summary>
	/// A directory is a logical reference to a hierarchical physical location containing files
	/// and directories.
	/// </summary>
    public interface Directory
    {
        DirectoryName Name { get; }

        Directory Parent { get; }
        bool HasParentDir { get; }
        
		/// <summary>
		/// Returns all of the files contained in the directory
		/// </summary>
		/// <returns></returns>
		IEnumerable<File> GetFiles();

		/// <summary>
		/// Returns all of the subdirectories in the directory
		/// </summary>
		/// <returns></returns>
        IEnumerable<Directory> GetDirectories();

		/// <summary>
		/// Checks if the directory exists
		/// </summary>
		/// <returns>True if the directory exists, otherwise false</returns>
		bool Exists();


		Directory GetChildDirectory(string name);

		File GetChildFile(string name);
        
		bool IsRoot();

        void CopyTo(DirectoryName path);
    }
}
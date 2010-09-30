// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Stact.Specs.FileSystem.FileSpecs
{
	using System;
	using System.IO;
	using System.Reflection;
    using Stact.FileSystem;
    using TestFramework;

    [Scenario]
    public class Creating_a_filename_from_a_string
    {
        string _location;
        FileName _fileName;
    	string _directory;

    	[Given]
        public void Setup()
        {
            _location = Assembly.GetExecutingAssembly().Location;
    		_directory = Path.GetDirectoryName(_location);
    		_directory = _directory.Substring(_directory.LastIndexOf('\\') + 1);

            _fileName = FileName.GetFileName(_location);
        }

        [Then]
        public void Should_get_the_directory_name()
        {
            _fileName.GetDirectoryName().GetName().ShouldEqual(_directory, StringComparison.InvariantCultureIgnoreCase);
        }

        [Then]
        public void Should_retrieve_the_filename()
        {
        	_fileName.GetName().ShouldEqual("Stact.Specs.dll", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
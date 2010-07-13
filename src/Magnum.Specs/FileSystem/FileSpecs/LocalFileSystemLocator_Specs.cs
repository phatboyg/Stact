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
namespace Magnum.Specs.FileSystem.FileSpecs
{
    using System.Reflection;
    using Magnum.FileSystem;
    using NUnit.Framework;
    using TestFramework;

    [TestFixture]
	public class Creating_a_filename_from_a_string
	{
		string _location;
		FileName _fileName;

		[TestFixtureSetUp]
		public void Setup()
		{
			_location = Assembly.GetExecutingAssembly().Location;

			_fileName = FileName.GetFileName(_location);
		}

		[Test]
		public void Should_get_the_directory_name()
		{
			_fileName.GetDirectoryName().GetName().ShouldEqual("bin");
		}

		[Test]
		public void Should_retrieve_the_filename()
		{
			_fileName.GetName().ShouldEqual("nu.Specs.dll");
		}
	}

	[TestFixture]
	public class Passing_a_filename_to_the_locator
	{
		FileName _fileName;

		[TestFixtureSetUp]
		public void Setup()
		{
			_fileName = FileName.GetFileName(Assembly.GetExecutingAssembly().Location);

			FileSystemLocator locator = new LocalFileSystemLocator();

			File file = locator.GetFile(_fileName);

			file.Exists().ShouldBeTrue();

			file.Name.GetPath().ShouldEqual(_fileName.GetPath());
		}

		[Test]
		public void Getting_a_directory_now()
		{
			FileSystemLocator locator = new LocalFileSystemLocator();

			Directory directory = locator.GetDirectory(DirectoryName.GetDirectoryName("sample.zip"));

			directory.Name.GetName().ShouldEqual("sample.zip");
		}

		[Test]
		public void Getting_busy()
		{
			FileSystemLocator locator = new LocalFileSystemLocator();

			File file = locator.GetFile(FileName.GetFileName("sample.zip/manifest.json"));

			file.Name.GetName().ShouldEqual("manifest.json");
		}
	}
}
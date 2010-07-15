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

    [Scenario]
	public class Passing_a_filename_to_the_locator_and_the_file_exists :
        Given_a_FileSystemLocator
	{
		FileName _fileName;
	    File _fileInQuestion;

		[When]
		public void When_you_get_a_file_that_exists()
		{
			_fileName = FileName.GetFileName(Assembly.GetExecutingAssembly().Location);

            _fileInQuestion = Locator.GetFile(_fileName);
		}

        [Then]
        public void The_file_should_exist()
        {
            _fileInQuestion.Exists().ShouldBeTrue();
        }

        [Then]
        public void The_file_path_should_match()
        {
            _fileInQuestion.Name.GetPath().ShouldEqual(_fileName.GetPath());
        }
	}

    [Scenario]
    public class Passing_a_filename_to_the_locator_and_the_file_DOESNT_exists :
        Given_a_FileSystemLocator
    {
        FileName _fileName;
        File _fileInQuestion;

        [When]
        public void When_you_get_a_file_that_DOESNT_exist()
        {
            _fileName = FileName.GetFileName(".\\doesntexist.txt");
            _fileInQuestion = Locator.GetFile(_fileName);
        }

        [Then, Ignore("We don't support getting non-existent files")]
        public void Getting_a_directory_now()
        {
            _fileInQuestion.Exists().ShouldBeFalse();
        }

        [Then, Ignore("we don't support getting non-existent files")]
        public void The_file_path_should_match()
        {
            _fileInQuestion.Name.GetPath().ShouldEqual(_fileName.GetPath());
        }

        /*
         Directory directory = locator.GetDirectory(DirectoryName.GetDirectoryName("somedir"));

            directory.Name.GetName().ShouldEqual("somedir");
         * */
    }

    [Scenario]
    public class Passing_a_directoryname_to_the_locator_and_the_directory_exists :
        Given_a_FileSystemLocator
    {
        DirectoryName _directoryName;
        Directory _directoryInQuestion;

        [When]
        public void When_you_get_a_directory_that_exists()
        {
            _directoryName = FileName.GetFileName(Assembly.GetExecutingAssembly().Location).GetDirectoryName();
            _directoryInQuestion = Locator.GetDirectory(_directoryName);
        }

        [Then]
        public void The_directory_should_exist()
        {
            _directoryInQuestion.Exists().ShouldBeTrue();
        }

        [Then]
        public void The_directory_path_should_match()
        {
            _directoryInQuestion.Name.GetPath().ShouldEqual(_directoryName.GetPath());
        }
    }

    [Scenario]
    public class Passing_a_directoryname_to_the_locator_and_the_directory_DOESNT_exists :
        Given_a_FileSystemLocator
    {
        DirectoryName _fileName;
        Directory _fileInQuestion;

        [When]
        public void When_you_get_a_directory_that_DOESNT_exist()
        {
            _fileName = DirectoryName.GetDirectoryName(".\\doesntexist");
            _fileInQuestion = Locator.GetDirectory(_fileName);
        }

        [Then, Ignore("We don't support getting non-existent directories")]
        public void Getting_a_directory_now()
        {
            _fileInQuestion.Exists().ShouldBeFalse();
        }

        [Then, Ignore("we don't support getting non-existent directories")]
        public void The_directory_path_should_match()
        {
            _fileInQuestion.Name.GetPath().ShouldEqual(_fileName.GetPath());
        }

        /*
         Directory directory = locator.GetDirectory(DirectoryName.GetDirectoryName("somedir"));

            directory.Name.GetName().ShouldEqual("somedir");
         * */
    }

}
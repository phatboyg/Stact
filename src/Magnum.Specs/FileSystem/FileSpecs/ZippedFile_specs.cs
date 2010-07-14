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
    using Magnum.FileSystem;
    using Magnum.FileSystem.Internal;
    using Magnum.FileSystem.Zip;
    using NUnit.Framework;
    using TestFramework;

    public class Given_a_zipped_file
    {
        string _zippedFile = @".\FileSystem\FileSpecs\sample.zip";

        ZipFileDirectory _zf;

        [Given]
        public void A_ZippedFile_Directory()
        {
            _zf = new ZipFileDirectory(new RelativePathName(_zippedFile));
        }

        [Then]
        public void The_path_of_the_zip_should_be_based_on_its_constructor()
        {
            Assert.AreEqual(".\\FileSystem\\FileSpecs\\sample.zip", _zf.Name.GetPath());
        }

        public ZipFileDirectory ZipFile
        {
            get
            {
                return _zf;
            }
        }
    }

    [Scenario]
    public class Accessing_an_existing_file_in_the_zip :
        Given_a_zipped_file
    {
        File _fileInQuestion;

        [When]
        public void Get_the_file()
        {
            _fileInQuestion = ZipFile.GetChildFile("MANIFEST.json");
        }

        [Then]
        public void File_should_exist()
        {
            _fileInQuestion.Exists().ShouldBeTrue();
        }

        [Then]
        public void Path_should_be_what()
        {
            Assert.AreEqual(".\\FileSystem\\FileSpecs\\sample.zip\\MANIFEST.json", _fileInQuestion.Name.GetPath());
        }

        [Then]
        public void Should_be_able_to_copy_file()
        {
            var data = _fileInQuestion.ReadToEnd();
            System.IO.File.WriteAllText(@"C:\Users\sellersd\Desktop\cm issue\test.txt",data);
        }
        //parent path
    }

    [Scenario]
    public class Accessing_an_existing_directory_in_the_zip :
        Given_a_zipped_file
    {
        Directory _directoryInQuestion;

        [When]
        public void Get_the_directory()
        {
            _directoryInQuestion = ZipFile.GetChildDirectory("lib");
        }

        [Then]
        public void Should_exist()
        {
            _directoryInQuestion.Exists().ShouldBeTrue();
        }

        [Then]
        public void Path_should_be_what()
        {
            Assert.AreEqual(".\\FileSystem\\FileSpecs\\sample.zip\\lib", _directoryInQuestion.Name.GetPath());
        }


        //parent path
    }

    [Scenario]
    public class Accessing_an_existing_nested_file :
        Given_a_zipped_file
    {
        File _fileInQuesiton;

        [When]
        public void Get_the_nested_file()
        {
            _fileInQuesiton = ZipFile.GetChildDirectory("lib").GetChildFile("yo.txt");
        }

        [Test]
        public void Should_exist()
        {
            _fileInQuesiton.Exists().ShouldBeTrue();
        }

    }
    
    [Scenario(Description="Zip won't zip an empty folder")]
    public class Accessing_an_empty_folder {}

    [Scenario]
    public class Accessing_a_nonexistent_file_in_the_zip :
        Given_a_zipped_file
    {
        File _fileInQuestion;

        [When]
        public void Get_the_file()
        {
            _fileInQuestion = ZipFile.GetChildFile("NOEXIST.json");
        }

        [Then]
        public void Should_not_exist()
        {
            _fileInQuestion.Exists().ShouldBeFalse();
        }

        [Then]
        public void The_path_should_be_correct()
        {
            Assert.AreEqual(".\\FileSystem\\FileSpecs\\sample.zip\\NOEXIST.json", _fileInQuestion.Name.GetPath());
        }
    }

    [Scenario]
    public class Accessing_a_nonexistent_directory_in_the_zip :
        Given_a_zipped_file
    {
        Directory _directoryInQuestion;

        [When]
        public void Get_the_file()
        {
            _directoryInQuestion = ZipFile.GetChildDirectory("NOEXIST");
        }

        [Then]
        public void Should_not_exist()
        {
            _directoryInQuestion.Exists().ShouldBeFalse();
        }

        [Then]
        public void The_path_should_be_correct()
        {
            Assert.AreEqual(".\\FileSystem\\FileSpecs\\sample.zip\\NOEXIST", _directoryInQuestion.Name.GetPath());
        }
    }

    [Scenario]
    public class Path_specs :
        Given_a_zipped_file
    {

        [Then]
        public void Should_convert_the_path_to_a_zip_path()
        {
            var result = ZippedPath.GetPathInsideZip("noexist.zip\\lib\\yo.txt");
            Assert.AreEqual("lib/yo.txt", result);
        }

        [Then]
        public void Should_get_the_name_of_the_zip_file()
        {
            var result = ZippedPath.GetZip("noexist.zip\\lib\\yo.txt");
            Assert.AreEqual("noexist.zip", result);
        }

        [Then]
        public void Should_get_the_parent_folder_of_the_file()
        {
            var result = ZippedPath.GetParentPath("noexist.zip\\lib\\yo.txt");
            Assert.AreEqual("lib", result);
        }

        [Then]
        public void Should_get_the_parent_folder_of_the_folder()
        {
            var result2 = ZippedPath.GetParentPath("noexist.zip\\test\\test2");
            Assert.AreEqual("test", result2);
        }

        [Then]
        public void Should_return_the_zip_if_a_root_item_is_asked_for_its_parent()
        {

            var result3 = ZippedPath.GetParentPath("noexist.zip\\test");
            Assert.AreEqual("noexist.zip", result3);


            var result4 = ZippedPath.GetParentPath("noexist.zip\\yo.txt");
            Assert.AreEqual("noexist.zip", result4);
        }
    }
}
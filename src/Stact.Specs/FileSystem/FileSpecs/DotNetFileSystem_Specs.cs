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
namespace Stact.Specs.FileSystem.FileSpecs
{
    using Stact.FileSystem;
    using NUnit.Framework;
    using TestFramework;

    [Scenario]
    public class The_fs_should_be_able_write_files
    {
        DotNetFileSystem fs;
        string filePath = @".\temp.txt";
        string contents = "hi123";

        [When]
        public void We_Write_a_file()
        {
            fs = new DotNetFileSystem();
            fs.Write(filePath, contents);       
        }

        [Then]
        public void File_contents_should_be_the_same()
        {
            Assert.AreEqual(fs.ReadToEnd(@".\temp.txt"), "hi123");


            fs.Write(@".\temp.txt", "hii");
            Assert.AreEqual(fs.ReadToEnd(@".\temp.txt"), "hii");

            fs.DeleteFile(@".\temp.txt");
        }
    }

    [Scenario]
    public class CopyAFile
    {
        DotNetFileSystem fs;
        string filePath = @".\temp.txt";
        string contents = "hi123";

        [When]
        public void We_Write_a_file()
        {
            fs = new DotNetFileSystem();
            fs.Write(filePath, contents);
            var f = new DotNetFile(FileName.GetFileName(@".\temp.txt"));
            f.CopyTo(FileName.GetFileName(@".\copy.txt"));
        }

        [Then]
        public void File_contents_should_be_the_same()
        {
            Assert.AreEqual("hi123", fs.ReadToEnd(@".\copy.txt"));

            fs.DeleteFile(@".\temp.txt");
            fs.DeleteFile(@".\copy.txt");
        }
    }
}
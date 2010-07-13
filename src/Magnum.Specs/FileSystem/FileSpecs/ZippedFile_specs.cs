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
    using Magnum.FileSystem.Internal;
    using Magnum.FileSystem.Zip;
    using NUnit.Framework;

    [TestFixture]
    public class Accessing_an_existing_zip_file
    {
        string _zippedFile = @".\FileSystem\FileSpecs\sample.zip";

        ZipFileDirectory _zf;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _zf = new ZipFileDirectory(new RelativePathName(_zippedFile));
        }

        [Test]
        public void GetChildDirectory()
        {
            var c = _zf.GetChildDirectory("lib");
            Assert.AreEqual("sample.zip\\lib", c.Name.GetPath());
        }


        [Test]
        public void GetChildFileExists()
        {
            var c = _zf.GetChildFile("MANIFEST.json");
            Assert.IsTrue(c.Exists());

            var c2 = _zf.GetChildDirectory("lib").GetChildFile("yo.txt");
            Assert.IsTrue(c2.Exists());
        }

        [Test]
        public void ZippedDirectoryExists()
        {
            var c = _zf.GetChildDirectory("lib");
            Assert.IsTrue(c.Exists());
            
        }

        [Test]
        public void GetChildFile()
        {
            var c = _zf.GetChildFile("MANIFEST.json");
            Assert.AreEqual("sample.zip\\MANIFEST.json", c.Name.GetPath());
        }

        [Test]
        public void PathWithRelative()
        {
            Assert.AreEqual(".\\FileSystem\\FileSpecs\\sample.zip", _zf.Name.GetPath());
        }

        [Test]
        public void ZipPathHelpers()
        {
            var result = ZippedPath.GetPathInsideZip("sample.zip\\lib\\yo.txt");
            Assert.AreEqual("lib/yo.txt", result);
        }

        [Test]
        public void MorePath()
        {
            var result = ZippedPath.GetZip("sample.zip\\lib\\yo.txt");
            Assert.AreEqual("sample.zip", result);
        }

        [Test]
        public void ParentPath()
        {
            var result = ZippedPath.GetParentPath("sample.zip\\lib\\yo.txt");
            Assert.AreEqual("lib", result);


            var result2 = ZippedPath.GetParentPath("sample.zip\\test\\test2");
            Assert.AreEqual("test", result2);


            var result3 = ZippedPath.GetParentPath("sample.zip\\test");
            Assert.AreEqual("sample.zip", result3);


            var result4 = ZippedPath.GetParentPath("sample.zip\\yo.txt");
            Assert.AreEqual("sample.zip", result4);
        }
    }
}
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
    using NUnit.Framework;

    [TestFixture]
    public class DotNetFileSystem_Specs
    {
        [Test]
        public void Overwrite_Files()
        {
            var fs = new DotNetFileSystem();

            fs.Write(@".\temp.txt", "hi123");
            Assert.AreEqual(fs.ReadToEnd(@".\temp.txt"), "hi123");


            fs.Write(@".\temp.txt", "hii");
            Assert.AreEqual(fs.ReadToEnd(@".\temp.txt"), "hii");

            fs.DeleteFile(@".\temp.txt");
        }
    }
}
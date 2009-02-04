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
namespace Magnum.Specs.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MbUnit.Framework;

    [TestFixture]
    public class FlattenedObject_Specs
    {
        [Test]
        public void Get_those_inner_objects_exposed()
        {
            List<FileEntry> entries = new List<FileEntry>
                                          {
                                              new FileEntry {ProcedureCode = "123", DiagnosisCode = "456", PolicyNumber = "ABC"},
                                              new FileEntry {ProcedureCode = "456", DiagnosisCode = "789", PolicyNumber = "ABC"},
                                              new FileEntry {ProcedureCode = "789", DiagnosisCode = "0AB", PolicyNumber = "DEF"},
                                              new FileEntry {ProcedureCode = "0AB", DiagnosisCode = "CDE", PolicyNumber = "GHI"},
                                          };

            var query = entries.Where(entry => entry.ProcedureCode == "456");

            Assert.AreEqual(1, query.Count());
        }

        [Test]
        public void What_about_pulling_the_objects_from_an_unknown_storage_type()
        {
            IQueryable<FileEntry> storage = new FileRepository<FileEntry>();

            var query = storage.Where(entry => entry.ProcedureCode == "456");

            Assert.AreEqual(1, query.Count());
        }
    }

    public class FileEntry
    {
        public string ProcedureCode { get; set; }
        public string DiagnosisCode { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PolicyNumber { get; set; }
    }
}
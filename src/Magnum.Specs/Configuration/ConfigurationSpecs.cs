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
namespace Magnum.Specs.Configuration
{

    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using Magnum.Configuration;
    using NUnit.Framework;
    using TestFramework;
    using Enumerable = System.Linq.Enumerable;

    [TestFixture]
    public class ConfigurationSpecs
    {
        #region Setup/Teardown

        [SetUp]
        public void Should_be_able_to_add_configuration_files_to_be_loaded()
        {
            _store = new ConfigurationStore();
            if (File.Exists("bob.json"))
                File.Delete("bob.json");

            File.AppendAllText("bob.json", CONF);
            _store.AddFile("bob.json");
        }

        #endregion

        ConfigurationStore _store;
        //TODO: Get rid of the UGLY 'Entries'
        const string CONF = @"{Entries:[{Key:""my-key"",Value:""my-value""}]}";

        [Test]
        public void Entries_should_be_accurate()
        {
            IEnumerable<ConfigurationEntry> entries = _store.GetEntries();
            ConfigurationEntry entry = Enumerable.First(entries);
            entry.Key.ShouldEqual("my-key");
            entry.Value.ShouldEqual("my-value");
        }

        [Test]
        public void Should_be_able_to_parse_the_file_and_return_entries()
        {
            IEnumerable<ConfigurationEntry> entries = _store.GetEntries();
            AssertionsForComparable.ShouldBeEqualTo(entries.Count(), 1);
        }

        [Test]
        public void The_name_should_be_viewable()
        {
            AssertionsForString.ShouldEqual(_store.FilesLoaded.First().FileName, "bob.json");
        }

        [Test]
        public void There_should_be_one_file()
        {
            AssertionsForObjects.ShouldEqual(_store.FilesLoaded.Count(), 1);
        }
    }
}
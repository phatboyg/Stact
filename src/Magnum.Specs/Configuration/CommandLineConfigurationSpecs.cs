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
    using Magnum.Configuration;
    using NUnit.Framework;
    using TestFramework;

    [TestFixture]
    public class CommandLineConfigurationSpecs
    {
        ConfigurationStore _store;

        [SetUp]
        public void Should_be_able_to_add_multiple_configuration_files_to_be_loaded()
        {
            _store = new ConfigurationStore();
            _store.AddCommandLine("-name:dru");
        }

        [Test]
        public void Should_show_one_registration()
        {
            _store.ProvidersLoaded.Count.ShouldEqual(1);
        }

        [Test]
        public void Should_parse_the_command_line()
        {
            var entries = _store.GetEntries();
            entries.Count().ShouldEqual(1);
        }

        [Test]
        public void Should_parse_the_command_line_correctly()
        {
            var entries = _store.GetEntries();
            var entry = entries.First();
            entry.Key.ShouldEqual("name");
            entry.Value.ShouldEqual("dru");
        }

    }
}
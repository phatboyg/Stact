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
    using Magnum.ValueProviders;
    using NUnit.Framework;
    using TestFramework;

    [Scenario]
    public class When_a_single_configuration_store_is_configured
    {
        ConfigurationStore _store;
    	ValueProvider _provider;

    	[When]
		public void A_single_configuration_store_is_configured()
        {
            _store = new ConfigurationStore();
            _store.AddCommandLine("-name:dru");

        	_provider = _store.GetValueProvider();
        }

        [Test]
        public void Should_parse_the_command_line_correctly()
        {
        	string resultValue = null;
        	_provider.GetValue("name", value =>
        		{
        			resultValue = value.ToString();

        			return true;
        		});

        	resultValue.ShouldEqual("dru");
        }

    }
}
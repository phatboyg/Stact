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
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using Magnum.Configuration;
	using Magnum.ValueProviders;
	using TestFramework;


	[Scenario]
	public class When_a_configuration_object_is_bound
	{
		MyConfiguration _configuration;
		ConfigurationBinder _configurationBinder;
		MultipleValueProvider _provider;

		[When]
		public void A_configuration_object_is_bound()
		{
			const string json = @"{ Name: ""phatboyg"", Password: ""default""}";
			var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

			string commandLineText = "-password:really_long_one --secure";

			var providers = new ValueProvider[]
				{
					new Magnum.ValueProviders.CommandLineValueProvider(commandLineText),
					new JsonValueProvider(jsonStream),
				};

			_provider = new MultipleValueProvider(providers);

			_configurationBinder = new ConfigurationBinder(_provider);

			_configuration = _configurationBinder.Bind<MyConfiguration>();
		}

		[Then]
		public void Should_have_name_value()
		{
			_configuration.Name.ShouldEqual("phatboyg");
		}

		[Then]
		public void Should_have_password_value()
		{
			_configuration.Password.ShouldEqual("really_long_one");
		}

		[Then]
		public void Should_have_secure_value()
		{
			_configuration.Secure.ShouldBeTrue();
		}

		[Then]
		public void Should_return_a_list_of_all_configuration()
		{
			var found = new Dictionary<string, object>();
			_provider.GetAll(found.Add);

			found.ContainsKey("password").ShouldBeTrue();
			found.ContainsKey("secure").ShouldBeTrue();
			found.ContainsKey("Name").ShouldBeTrue();
			found.ContainsKey("Password").ShouldBeTrue();
		}


		public class MyConfiguration
		{
			public string Name { get; set; }
			public string Password { get; set; }
			public bool Secure { get; set; }
		}
	}
}
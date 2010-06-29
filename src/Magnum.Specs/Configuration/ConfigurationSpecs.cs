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
namespace Magnum.Specs.Configuration
{
	using System.IO;
	using Magnum.Configuration;
	using Magnum.ValueProviders;
	using NUnit.Framework;
	using TestFramework;


	[TestFixture]
	public class ConfigurationSpecs
	{
		[SetUp]
		public void Should_be_able_to_add_configuration_files_to_be_loaded()
		{
			_store = new ConfigurationStore();
			if (File.Exists("bob.json"))
				File.Delete("bob.json");

			File.AppendAllText("bob.json", CONF);
			_store.AddJsonFile("bob.json");

			_provider = _store.GetValueProvider();
		}

		[Test]
		public void Should_parse_the_first_value()
		{
			string resultValue = null;
			_provider.GetValue("key", value =>
				{
					resultValue = value.ToString();

					return true;
				});

			resultValue.ShouldEqual("my-key");
		}

		[Test]
		public void Should_parse_the_second_value()
		{
			string resultValue = null;
			_provider.GetValue("value", value =>
				{
					resultValue = value.ToString();

					return true;
				});

			resultValue.ShouldEqual("my-value");
		}

		ConfigurationStore _store;
		ValueProvider _provider;
		const string CONF = @"{Key:""my-key"",Value:""my-value""}";
	}
}
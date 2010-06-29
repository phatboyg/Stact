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
	using NUnit.Framework;
	using TestFramework;


	[TestFixture]
	public class When_a_single_json_configuration_file_is_loaded
	{
		ConfigurationBinder _binder;

		[When]
		public void A_single_json_configuration_file_is_loaded()
		{
			if (File.Exists("bob.json"))
				File.Delete("bob.json");

			const string conf = @"{""my-key"": ""my-value""}";

			File.AppendAllText("bob.json", conf);

			_binder = ConfigurationBinderFactory.New(x =>
				{
					// a single json file
					x.AddJsonFile("bob.json");
				});
		}

		[Then]
		public void Should_parse_the_first_value()
		{
			string value = _binder.GetValue("my-key");

			value.ShouldEqual("my-value");
		}
	}
}
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
	using Magnum.Logging;
	using TestFramework;


	[Scenario]
	public class When_configuration_values_are_read_from_a_series_of_json_files
	{
		ConfigurationBinder _binder;

		[When]
		public void Configuration_values_are_read_from_a_series_of_json_files()
		{
			TraceLogProvider.Configure(LogLevel.Debug);

			if (File.Exists("global.json"))
				File.Delete("global.json");
			if (File.Exists("local.json"))
				File.Delete("local.json");

			const string globalConf = @"{ key1: ""global-value-1"", key2: ""global-value-2""}";
			const string localConf = @"{ key1: ""local-value-1""}";


			File.AppendAllText("global.json", globalConf);
			File.AppendAllText("local.json", localConf);

			_binder = ConfigurationBinderFactory.New(x =>
				{
					// most-specific to least specific
					// I assume that is reasonable

					x.AddJsonFile("local.json");
					x.AddJsonFile("global.json");
				});
		}

		[Then]
		public void A_local_value_should_override_a_global_value()
		{
			string value = _binder.GetValue("key1");
			value.ShouldEqual("local-value-1");
		}
	}
}
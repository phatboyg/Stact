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
	using Magnum.Configuration;
	using NUnit.Framework;
	using TestFramework;


	[Scenario]
	public class When_using_the_configuration_command_line_value_provider
	{
		ConfigurationBinder _binder;

		[When]
		public void Using_the_configuration_command_line_value_provider()
		{
			_binder = ConfigurationBinderFactory.New(x => { x.AddCommandLine("-name:dru"); });
		}

		[Test]
		public void Should_parse_the_command_line_correctly()
		{
			string value = _binder.GetValueAsString("name");

			value.ShouldEqual("dru");
		}
	}
}
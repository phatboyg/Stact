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
namespace Magnum.Specs.ValueProviders
{
	using System.Collections.Generic;
	using System.Linq;
	using Magnum.CommandLineParser;
	using Magnum.ValueProviders;
	using NUnit.Framework;
	using TestFramework;


	[Scenario]
	public class Given_a_command_line
	{
		protected string CommandLineText;
		protected IEnumerable<ICommandLineElement> Elements;

		[Given]
		public void A_command_line()
		{
			CommandLineText = "-name:phatboyg -password:really_long_one --secure";

			Elements = new MonadicCommandLineParser().Parse(CommandLineText);
		}

		[Then]
		public void Should_contain_the_name()
		{
			Elements.Where(x => x is IDefinitionElement)
				.Cast<IDefinitionElement>()
				.Where(x => x.Key == "name")
				.Where(x => x.Value == "phatboyg")
				.Any()
				.ShouldBeTrue();
		}


		[Then]
		public void Should_contain_the_password()
		{
			Elements.Where(x => x is IDefinitionElement)
				.Cast<IDefinitionElement>()
				.Where(x => x.Key == "password")
				.Where(x => x.Value == "really_long_one")
				.Any()
				.ShouldBeTrue();
		}

		[Then]
		public void Should_contain_the_secure_switch()
		{
			Elements.Where(x => x is ISwitchElement)
				.Cast<ISwitchElement>()
				.Where(x => x.Key == "secure")
				.Where(x => x.Value)
				.Any()
				.ShouldBeTrue();
		}
	}


	[Scenario]
	public class When_a_value_is_defined_in_the_command_line :
		Given_a_command_line
	{
		ValueProvider _provider;

		[When]
		public void A_value_is_defined_in_the_command_line()
		{
			_provider = new CommandLineValueProvider(CommandLineText);
		}

		[Test]
		public void Should_return_the_name()
		{
			string value = null;
			bool found = _provider.GetValue("name", x =>
				{
					value = x.ToString();
					return true;
				});

			found.ShouldBeTrue();
			value.ShouldEqual("phatboyg");
		}

		[Test]
		public void Should_return_the_password()
		{
			string value = null;
			bool found = _provider.GetValue("password", x =>
				{
					value = x.ToString();
					return true;
				});

			found.ShouldBeTrue();
			value.ShouldEqual("really_long_one");
		}
		[Test]
		public void Should_return_the_secure_switch()
		{
			bool value = false;
			bool found = _provider.GetValue("secure", x =>
				{
					value = x.GetType() == typeof(bool) ? (bool)x : bool.Parse(x.ToString());
					return true;
				});

			found.ShouldBeTrue();
			value.ShouldBeTrue();
		}
	}
}
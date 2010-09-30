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
namespace Stact.Specs.Configuration
{
	using Stact.Configuration;
	using TestFramework;


	[Scenario]
	public class When_retrieving_a_nested_value
	{
		ConfigurationBinder _binder;
		OuterLevel _configuration;

		[When]
		public void A_configuration_object_is_bound()
		{
			const string json = @"{ Inner: { Value: 47 } }";

			_binder = ConfigurationBinderFactory.New(x => { x.AddJson(json); });

			_configuration = _binder.Bind<OuterLevel>();
		}

		[Then]
		public void Should_have_name_value()
		{
			_configuration.Inner.Value.ShouldEqual(47);
		}


		public class InnerLevel
		{
			public int Value { get; set; }
		}


		class OuterLevel
		{
			public OuterLevel()
			{
				Inner = new InnerLevel();
			}

			public InnerLevel Inner { get; set; }
		}
	}
}
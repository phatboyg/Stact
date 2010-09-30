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
namespace Stact.Specs.Serialization
{
	using System.Collections.Generic;
	using TestFramework;

	[Scenario]
	public class When_serializing_an_idictionary_of_values :
		With_the_fast_text_serializer
	{
		private string _body;
		private IDictionary<string,string> _value;

		[When]
		public void A_dictionary_of_strings_is_serialized()
		{
			_value = new Dictionary<string, string>
				{
					{"1st", "First"},
					{"2nd", "Second"},
					{"3rd", "Third"},
				};

			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("{1st:First,2nd:Second,3rd:Third}");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<IDictionary<string,string>>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_dictionary_of_values :
		With_the_fast_text_serializer
	{
		private string _body;
		private Dictionary<string,string> _value;

		[When]
		public void A_dictionary_of_strings_is_serialized()
		{
			_value = new Dictionary<string, string>
				{
					{"1st", "First"},
					{"2nd", "Second"},
					{"3rd", "Third"},
				};

			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("{1st:First,2nd:Second,3rd:Third}");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<Dictionary<string,string>>(_body);

			value.ShouldEqual(_value);
		}
	}
}
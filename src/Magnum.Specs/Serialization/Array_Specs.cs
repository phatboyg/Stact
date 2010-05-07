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
namespace Magnum.Specs.Serialization
{
	using NUnit.Framework;
	using TestFramework;

	[Scenario]
	public class When_serializing_an_array_of_ints :
		With_the_fast_text_serializer
	{
		private string _body;
		private int[] _value;

		[When]
		public void An_array_of_int_is_serialized()
		{
			_value = new[] {8, 6, 7, 5, 3, 0, 9};
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("[8,6,7,5,3,0,9]");
		}

		[Then, Ignore("Still working on deserializing the array bits")]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<int[]>(_body);

			value.ShouldEqual(_value);
		}
	}
}
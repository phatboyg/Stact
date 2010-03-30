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
namespace Magnum.Web.Specs
{
	using System.Collections.Generic;
	using Binding;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TestFramework;
	using ValueProviders;

	[TestFixture]
	public class Binding_a_dictionary_to_an_object
	{
		private Dictionary<string, object> _dictionary;
		private BinderTestClass _result;

		[TestFixtureSetUp]
		public void Setup()
		{
			_dictionary = new Dictionary<string, object>
				{
					{"StringValue", "Value"},
					{"IntValue", "47"},
				};

			ModelBinder binder = new FastModelBinder();


			var binderContext = MockRepository.GenerateMock<ModelBinderContext>();
			//binderContext.Stub(x => x.GetValue("StringValue", )).Return(new DictionaryValueProvider(_dictionary));

			object obj = binder.Bind(typeof (BinderTestClass), binderContext);

			_result = obj as BinderTestClass;
		}

		private class BinderTestClass
		{
			public string StringValue { get; set; }
			public int IntValue { get; set; }
		}

		[Test]
		public void Should_not_return_a_null_object()
		{
			_result.ShouldNotBeNull();
		}

		[Test]
		public void Should_properly_bind_a_string_value()
		{
			_result.StringValue.ShouldEqual("Value");
		}

		[Test]
		public void Should_properly_bind_a_integer_value()
		{
			_result.IntValue.ShouldEqual(47);
		}
	}
}
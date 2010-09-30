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
	using Stact.Logging;
	using Stact.Serialization;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Using_the_fast_text_serializer
	{
		private Serializer _serializer;

		[SetUp]
		public void Setup()
		{
			TraceLogger.Configure(LogLevel.Debug);

			_serializer = new FastTextSerializer();
		}

		[Test]
		public void Should_have_an_easy_to_use_serialization_method()
		{
			string text = _serializer.Serialize("Hello");

			text.ShouldEqual("Hello");
		}

		[Test]
		public void Should_have_an_easy_to_use_serialization_method_that_works()
		{
			string text = _serializer.Serialize(12345);

			text.ShouldEqual("12345");
		}

		[Test]
		public void Should_escape_a_string_if_it_has_a_quoted_character_in_it()
		{
			string text = _serializer.Serialize("[Amazing]");

			text.ShouldEqual("\"[Amazing]\"");
		}
	}
}
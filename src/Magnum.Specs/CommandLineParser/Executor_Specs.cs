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
namespace Magnum.Specs.CommandLineParser
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;
	using Magnum.CommandLineParser;
	using Magnum.Reflection;
	using NUnit.Framework;

	[TestFixture]
	public class Specifying_a_convention_for_commands :
		BehaviorTest
	{
		protected string CommandLine { get; set; }
		protected IEnumerable<ICommandLineElement> Elements { get; set; }

		protected override void Given()
		{
			CommandLine = "-from \"source.txt\" -to \"destination.txt\" -overwrite";
		}

		protected override void When()
		{
			Elements = new MonadicCommandLineParser().Parse(CommandLine);
		}

		[Test, Ignore]
		public void Should_copy_the_arguments_into_the_class()
		{
			object arguments = ClassFactory.New(typeof (MoveCommandArguments));

			var properties = arguments.GetType()
				.GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.Where(x => x.GetGetMethod(true) != null)
				.Where(x => x.GetSetMethod(true) != null)
				.Select(x => new FastProperty(x));

			properties
				.Each(property =>
					{
						object value = Elements.GetDefinitionElement(property.Property.PropertyType, property.Property.Name);
						if (value != null)
						{
							property.Set(arguments, value);
							return;
						}
					});


			var test = (MoveCommandArguments) arguments;

			Assert.AreEqual("source.txt", test.From);
			Assert.AreEqual("destination.txt", test.To);
			Assert.IsTrue(test.Overwrite);
		}
	}

	public static class somethingoiuweoiuroweiru
	{
		public static string GetDefinitionElement(this IEnumerable<ICommandLineElement> elements, string key)
		{
			return elements.GetDefinitionElement(key, StringComparison.CurrentCultureIgnoreCase);
		}

		public static object GetDefinitionElement(this IEnumerable<ICommandLineElement> elements, Type resultType, string key)
		{
			string value = elements.GetDefinitionElement(key);

			if (!typeof(Nullable<>).IsAssignableFrom(resultType) && value == null)
				return null;

			object result = value;

			var converter = TypeDescriptor.GetConverter(typeof (string));

			if(converter.CanConvertTo(resultType))
				return converter.ConvertTo(value, resultType);

			return result;
		}

		public static string GetDefinitionElement(this IEnumerable<ICommandLineElement> elements, string key, StringComparison comparison)
		{
			return elements
				.Where(x => x is DefinitionElement)
				.Select(x => (DefinitionElement) x)
				.Where(x => string.Compare(x.Key, key, comparison) == 0)
				.Select(x => x.Value)
				.DefaultIfEmpty(null)
				.SingleOrDefault();
		}
	}
}
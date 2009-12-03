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
namespace Magnum.Generator
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Reflection;
	using CollectionExtensions;

	public class ObjectGenerator
	{
		private static ObjectGenerator _current;

		private readonly Dictionary<Type, IObjectGenerator> _generators;

		private ObjectGenerator()
		{
			_generators = new Dictionary<Type, IObjectGenerator>();
		}

		public static ObjectGenerator Current
		{
			get
			{
				if (_current == null)
					_current = new ObjectGenerator();

				return _current;
			}
		}

		public static object Create(Type type)
		{
			return Current._generators.Retrieve(type, () =>
				{
					const BindingFlags flags = BindingFlags.Static | BindingFlags.Public;

					return (IObjectGenerator) typeof (ObjectGenerator<>).MakeGenericType(type)
					                          	.GetProperty("Current", flags)
					                          	.GetValue(null, flags, null, null, CultureInfo.InvariantCulture);
				}).Create();
		}
	}
}
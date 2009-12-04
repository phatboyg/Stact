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
	using System.Linq;
	using System.Runtime.Serialization;

	public class ObjectGeneratorException :
		Exception
	{
		public ObjectGeneratorException()
		{
		}

		public ObjectGeneratorException(Type type, string message, params Type[] argumentTypes)
			: base(FormatMessage(type, message, argumentTypes))
		{
			Type = type;
			ArgumentTypes = new List<Type>(argumentTypes);
		}

		public ObjectGeneratorException(Type type, string message, Exception innerException, params Type[] argumentTypes)
			: base(FormatMessage(type, message, argumentTypes), innerException)
		{
			Type = type;
			ArgumentTypes = new List<Type>(argumentTypes);
		}

		protected ObjectGeneratorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public Type Type { get; set; }
		public IEnumerable<Type> ArgumentTypes { get; set; }

		private static string FormatMessage(Type type, string message, Type[] argumentTypes)
		{
			return string.Format("An object of type {0} could not be created. {1}{2}", type.Name, message,
				string.Join(", ", argumentTypes.Select(x => x.Name).ToArray()));
		}
	}
}
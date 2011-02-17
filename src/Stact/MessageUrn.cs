// Copyright 2010 Chris Patterson
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
namespace Stact
{
	using System;
	using System.Text;


	public static class MessageUrn
	{
		public static Type GetMessageType(string urn)
		{
			return GetMessageType(new Uri(urn));
		}

		public static Type GetMessageType(Uri urn)
		{
			if (urn.Segments.Length == 0)
				return null;

			string[] names = urn.Segments[0].Split(':');
			if (names[0] != "message")
				return null;

			if (names.Length < 2)
				return null;

			string typeName = names[1];

			if (names.Length == 2)
				typeName = names[1];
			else if (names.Length == 3)
				typeName = names[2] + "." + names[1] + ", " + names[2];
			else if (names.Length >= 4)
				typeName = names[2] + "." + names[1] + ", " + names[3];

			Type messageType = Type.GetType(typeName, true, true);

			return messageType;
		}

		public static Uri GetUrn(Type type)
		{
			var sb = new StringBuilder("urn:message:");

			return new Uri(GetMessageName(sb, type, true));
		}

		public static string GetMessageName(Type type)
		{
			return GetMessageName(new StringBuilder(), type, true);
		}

		static string GetMessageName(StringBuilder sb, Type type, bool includeScope)
		{
			if(type.IsNested)
			{
				GetMessageName(sb, type.DeclaringType, false);
				sb.Append('+');
			}

			if (type.IsGenericType)
			{
				sb.Append(type.GetGenericTypeDefinition().FullName);
				sb.Append('[');

				Type[] arguments = type.GetGenericArguments();
				for (int i = 0; i < arguments.Length; i++)
				{
					if (i > 0)
						sb.Append(',');

					sb.Append('[');
					GetMessageName(sb, arguments[i], true);
					sb.Append(']');
				}

				sb.Append(']');
			}
			else
			{
				sb.Append(type.Name);
			}

			if (includeScope && type.Namespace != null)
			{
				sb.Append(':');

				string ns = type.Namespace;
				sb.Append(ns);

				string assembly = type.Assembly.FullName;
				if (!string.IsNullOrEmpty(assembly))
				{
					string assemblyName = assembly.Substring(0, assembly.IndexOf(','));
					if (assemblyName != ns)
					{
						sb.Append(':');
						sb.Append(assemblyName);
					}
				}
			}

			return sb.ToString();
		}
	}
}
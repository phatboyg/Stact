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
namespace Magnum.Parsers
{
	using System.Collections.Generic;
	using System.Linq;

	public static class ExtensionsToRangeParser
	{
		public static string ToRangeString(this IEnumerable<IRangeElement> elements)
		{
			return string.Join(";", elements.Select(x => x.ToString()).ToArray());
		}

		public static bool Includes(this IEnumerable<IRangeElement> elements, IRangeElement find)
		{
			foreach (IRangeElement element in elements)
			{
				if(element.Includes(find) && !ReferenceEquals(element,find))
					return true;
			}

			return false;
		}

		public static IEnumerable<IRangeElement> Optimize(this IEnumerable<IRangeElement> elements)
		{
			var results = new List<IRangeElement>();

			foreach (IRangeElement element in elements)
			{
				if(results.Contains(element))
					continue;

				if(results.Includes(element))
					continue;

				results.Add(element);
			}

			foreach (IRangeElement result in results)
			{
				if(!results.Includes(result))
					yield return result;
			}
		}
	}
}
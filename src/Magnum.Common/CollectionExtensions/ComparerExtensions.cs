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
namespace Magnum.Common.CollectionExtensions
{
	using System.Collections.Generic;

	public static class ComparerExtensions
	{
		public static IComparer<T> Reverse<T>(this IComparer<T> original)
		{
			ReverseComparer<T> originalAsReverse = original as ReverseComparer<T>;
			if (originalAsReverse != null)
				return originalAsReverse.OriginalComparer;

			return new ReverseComparer<T>(original);
		}

		public static IComparer<T> ThenBy<T>(this IComparer<T> firstComparer, IComparer<T> secondComparer)
		{
			return new LinkedComparer<T>(firstComparer, secondComparer);
		}
	}
}
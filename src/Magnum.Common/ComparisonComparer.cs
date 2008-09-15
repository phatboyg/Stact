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
namespace Magnum.Common
{
	using System;
	using System.Collections.Generic;
	using ObjectExtensions;

	public class ComparisonComparer<T> : IComparer<T>
	{
		readonly Comparison<T> _comparison;

		public ComparisonComparer(Comparison<T> comparison)
		{
			comparison.MustNotBeNull("comparison");

			_comparison = comparison;
		}

		public int Compare(T x, T y)
		{
			return _comparison(x, y);
		}

		public static Comparison<T> CreateComparison(IComparer<T> comparer)
		{
			comparer.MustNotBeNull("comparer");

			return (x, y) => comparer.Compare(x, y);
		}
	}
}
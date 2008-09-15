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
	public class RangeBuilder<T>
	{
		internal bool _includeLowerBound;
		internal bool _includeUpperBound;
		internal T _lowerBound;
		internal T _upperBound;

		public RangeBuilder(T lowerBound)
		{
			_lowerBound = lowerBound;
			_upperBound = lowerBound;
			_includeLowerBound = true;
			_includeUpperBound = true;
		}

		/// <summary>
		/// Specifies the upper bound for the range
		/// </summary>
		/// <param name="upperBound"></param>
		/// <returns></returns>
		public RangeBuilder<T> Through(T upperBound)
		{
			_upperBound = upperBound;
			return this;
		}

		public RangeBuilder<T> IncludeLowerBound()
		{
			_includeLowerBound = true;
			return this;
		}

		public RangeBuilder<T> IncludeUpperBound()
		{
			_includeUpperBound = true;
			return this;
		}

		public RangeBuilder<T> ExcludeLowerBound()
		{
			_includeLowerBound = false;
			return this;
		}

		public RangeBuilder<T> ExcludeUpperBound()
		{
			_includeUpperBound = false;
			return this;
		}

		public static implicit operator Range<T>(RangeBuilder<T> builder)
		{
			return new Range<T>(builder._lowerBound, builder._upperBound, builder._includeLowerBound, builder._includeUpperBound);
		}
	}

	public static class RangeBuilderExt
	{
		public static RangeBuilder<T> Through<T>(this T start, T end)
		{
			return new RangeBuilder<T>(start).Through(end);
		}
	}
}
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
	public class LessThanElement :
		IRangeElement
	{
		public string End { get; private set; }

		public LessThanElement(string end)
		{
			End = end;
		}

		public override string ToString()
		{
			return string.Format("-{0}", End);
		}

		public bool Includes(IRangeElement element)
		{
			if (element == null)
				return false;

			if (element is StartsWithElement)
				return Includes((StartsWithElement) element);

			if (element is RangeElement)
				return Includes((RangeElement) element);

			return false;
		}

		private bool Includes(StartsWithElement element)
		{
			if (element.Start.CompareTo(End) <= 0)
				return true;

			return false;
		}

		private bool Includes(RangeElement element)
		{
			if (element.End.CompareTo(End) <= 0)
				return true;

			return false;
		}

		public bool Equals(LessThanElement other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.End, End);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (LessThanElement)) return false;
			return Equals((LessThanElement)obj);
		}

		public override int GetHashCode()
		{
			return (End != null ? End.GetHashCode() : 0);
		}
	}
}
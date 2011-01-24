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
namespace Stact.Internal
{
	using Functional;


	public class RightImpl<TLeft, TRight> :
		Either<TLeft, TRight>
	{
		readonly TRight _value;

		public RightImpl(TRight value)
		{
			_value = value;
		}

		public TRight Value
		{
			get { return _value; }
		}

		public LeftProjection<TLeft, TRight> Left
		{
			get { return new LeftProjectionImpl<TLeft, TRight>(this); }
		}

		public RightProjection<TLeft, TRight> Right
		{
			get { return new RightProjectionImpl<TLeft, TRight>(this); }
		}

		public bool IsLeft
		{
			get { return false; }
		}

		public bool IsRight
		{
			get { return true; }
		}
	}
}
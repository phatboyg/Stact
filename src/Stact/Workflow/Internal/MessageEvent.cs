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
namespace Stact.Workflow.Internal
{
	using Magnum.Extensions;


	public class MessageEvent<TBody> :
		SimpleEvent,
		Event<TBody>
	{
		public MessageEvent(string name)
			: base(name)
		{
		}

		public override string ToString()
		{
			return Name + "<" + typeof(TBody).ToShortTypeName() + ">";
		}

		public bool Equals(MessageEvent<TBody> other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return Equals(other.Name, Name);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			return Equals(obj as MessageEvent<TBody>);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
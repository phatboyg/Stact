// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Magnum.ForNHibernate.Specs.Channels
{
	using System;


	public class PreviousValue
	{
		public PreviousValue(DateTime updateDate, decimal value)
		{
			UpdateDate = updateDate;
			Value = value;
		}

		public PreviousValue()
		{
		}

		public virtual int Id { get; private set; }
		public virtual decimal Value { get; set; }
		public virtual DateTime UpdateDate { get; set; }

		public virtual bool Equals(PreviousValue other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return other.Id == Id;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(PreviousValue))
				return false;
			return Equals((PreviousValue)obj);
		}

		public override int GetHashCode()
		{
			return Id;
		}
	}
}
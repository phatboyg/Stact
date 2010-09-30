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
namespace Stact.ForNHibernate.Specs.Auditing.Complete
{
	using System;


	[Serializable]
	public class AuditKey
	{
		public virtual Guid SessionId { get; set; }
		public virtual long Timestamp { get; set; }
		public virtual int TestInstanceId { get; set; }

		public virtual bool Equals(AuditKey other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return other.SessionId.Equals(SessionId) && other.Timestamp == Timestamp && other.TestInstanceId == TestInstanceId;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(AuditKey))
				return false;
			return Equals((AuditKey)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = SessionId.GetHashCode();
				result = (result*397) ^ Timestamp.GetHashCode();
				result = (result*397) ^ TestInstanceId;
				return result;
			}
		}
	}
}
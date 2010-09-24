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
	using System.Collections.Generic;
	using Fibers;
	using Magnum.Channels;


	public class TestInstance
	{
		Fiber _fiber;

		public TestInstance(int id)
			: this()
		{
			Id = id;
		}

		protected TestInstance()
		{
			_fiber = new SynchronousFiber();

			UpdateValueChannel = new ConsumerChannel<UpdateValue>(_fiber, HandleUpdateValue);
			PreviousValues = new List<PreviousValue>();
		}

		public TestInstance(int id, decimal value)
			: this()
		{
			Id = id;
			Value = value;
		}

		public virtual Channel<UpdateValue> UpdateValueChannel { get; private set; }

		public virtual int Id { get; private set; }
		public virtual decimal Value { get; set; }

		public virtual IList<PreviousValue> PreviousValues { get; set; }

		void HandleUpdateValue(UpdateValue m)
		{
			PreviousValues.Add(new PreviousValue(SystemUtil.UtcNow, Value));
			Value += m.Value;
			m.MarkAsReceived();
		}

		public virtual bool Equals(TestInstance other)
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
			if (obj.GetType() != typeof(TestInstance))
				return false;
			return Equals((TestInstance)obj);
		}

		public override int GetHashCode()
		{
			return Id;
		}
	}


	public class UpdateValue
	{
		static Action<UpdateValue> _received = _ => { };


		public UpdateValue(int id, decimal value)
		{
			Id = id;
			Value = value;
		}

		public int Id { get; private set; }
		public decimal Value { get; private set; }

		public static void SetReceivedCallback(Action<UpdateValue> callback)
		{
			_received = callback;
		}

		public void MarkAsReceived()
		{
			_received(this);
		}
	}
}
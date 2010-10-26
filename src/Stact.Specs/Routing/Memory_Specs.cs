namespace Stact.Specs
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Magnum.TestFramework;


	[Scenario]
	public class Using_memory_from_a_session
	{
		[Then]
		public void Should_not_match_if_value_has_been_evicted()
		{
			var session = new SessionImpl();

			var sessionValue = new SessionValue("Hello");

			session.Add(sessionValue);

			var memoryImpl = new MemoryImpl();

			memoryImpl.Add(sessionValue);

			sessionValue.Evict();

			memoryImpl.Any().ShouldBeFalse();
		}
	}

	public class MemoryImpl :
		IEnumerable<SessionValue>
	{
		readonly IList<SessionValue> _values;

		public MemoryImpl()
		{
			_values = new List<SessionValue>();
		}

		public void Add(SessionValue memoryValue)
		{
			_values.Add(memoryValue);
		}

		public IEnumerator<SessionValue> GetEnumerator()
		{
			foreach (var memoryValue in _values)
			{
				if (memoryValue.IsAvailable)
					yield return memoryValue;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}


	public class SessionValue
	{
		readonly object _value;
		bool _available = true;

		public SessionValue(object value)
		{
			_value = value;
		}

		public bool Equals(SessionValue other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return Equals(other._value, _value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(SessionValue))
				return false;
			return Equals((SessionValue)obj);
		}

		public override int GetHashCode()
		{
			return (_value != null ? _value.GetHashCode() : 0);
		}

		public bool IsAvailable
		{
			get { return _available; }
		}

		public void Evict()
		{
			_available = false;
		}
	}

	public class SessionImpl
	{
		readonly HashSet<SessionValue> _values;

		public SessionImpl()
		{
			_values = new HashSet<SessionValue>();
		}

		public void Add(SessionValue sessionValue)
		{
			_values.Add(sessionValue);

		}
	}
}
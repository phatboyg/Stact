namespace Magnum.Common.Tests
{
	using System;
	using System.Text;
	using Reflection;

	public class ExtendedFunctionTimer<T> :
		FunctionTimer
		where T : class, new()
	{
		private readonly T _values;

		public ExtendedFunctionTimer(string description, Action<string> action)
			: base(description, action)
		{
			_values = new T();
		}

		public T Values
		{
			get { return _values; }
		}

		protected override void OutputAdditionalValues(StringBuilder sb)
		{
			var items = ReflectionCache<T>.List(_values);

			foreach (object o in items)
			{
				sb.Append(' ').Append(o ?? "null");
			}
		}
	}
}
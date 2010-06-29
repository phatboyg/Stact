namespace Magnum.ValueProviders
{
	using System;
	using System.Collections.Generic;
	using Logging;

	/// <summary>
	/// Makes wrapping another value provider easy, including logging of values as they are utilized
	/// </summary>
	public abstract class ValueProviderDecorator :
		ValueProvider
	{
		static readonly ILogger _log = Logger.GetLogger<ValueProvider>();
		static readonly HashSet<string> _obscuredKeys;
		readonly ValueProvider _provider;

		static ValueProviderDecorator()
		{
			_obscuredKeys = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
				{
					"password",
					"pw",
				};
		}

		protected ValueProviderDecorator(ValueProvider provider)
		{
			_provider = provider;
		}

		protected abstract string ProviderName { get; }

		public bool GetValue(string key, Func<object, bool> matchingValueAction)
		{
			object returnedValue = null;
			bool result = _provider.GetValue(key, value =>
				{
					returnedValue = ObscureValueIfNecessary(key, value);

					return matchingValueAction(value);
				});

			if (result)
				_log.Debug(x => x.Write("Read value for {0} from {2}: {1}", key, returnedValue, ProviderName));

			return result;
		}

		public bool GetValue(string key, Func<object, bool> matchingValueAction, Action missingValueAction)
		{
			object returnedValue = null;
			bool result = _provider.GetValue(key, value =>
				{
					returnedValue = ObscureValueIfNecessary(key, value);

					return matchingValueAction(value);
				}, missingValueAction);

			if (result)
				_log.Debug(x => x.Write("Read value for {0} from {2}: {1}", key, returnedValue, ProviderName));

			return result;
		}

		public void GetAll(Action<string, object> valueAction)
		{
			_provider.GetAll(valueAction);
		}

		static string ObscureValueIfNecessary(string key, object value)
		{
			if (value == null)
				return "(null)";

			string text = value.ToString();

			if (_obscuredKeys.Contains(key))
				return new string('*', text.Length);

			if (text.Length > 100)
				return text.Substring(0, 100) + "... (" + text.Length + " bytes)";

			return text;
		}
	}
}
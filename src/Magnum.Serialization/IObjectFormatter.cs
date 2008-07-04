namespace Magnum.Serialization
{
	using System;
	using System.Reflection;

	public interface IObjectFormatter : IDisposable
	{
		void Start();
		void Stop();
		void StartObject(Type type);
		void EndObject(Type type);
		void WriteField(FieldInfo info, string value);
		void WriteString(FieldInfo info, string value);
	}
}
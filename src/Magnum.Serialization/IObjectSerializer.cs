namespace Magnum.Serialization
{
	using System;

	public interface IObjectSerializer : IDisposable
	{
		void Serialize<T>(T obj) where T : class;
	}
}
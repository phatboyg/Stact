namespace Magnum.Serialization.Yaml
{
    using System;
    using Magnum.Serialization;


    public interface YamlTypeSerializer
    {
        void Serialize<T>(T obj, Action<string> output);
        T Deserialize<T>(string text);
    }

    public class YamlTypeSerializer<T> :
        YamlTypeSerializer,
        TypeSerializer<T>
    {
		readonly TypeReader<T> _deserializer;
		readonly TypeWriter<object> _serializer;
		readonly TypeSerializer<T> _typeSerializer;

        public YamlTypeSerializer(TypeSerializer<T> typeSerializer)
		{
			_typeSerializer = typeSerializer;

			TypeWriter<T> serialize = typeSerializer.GetWriter();
			_serializer = (value, output) => { serialize((T)value, output); };

			_deserializer = typeSerializer.GetReader();
		}

		public void Serialize<TObject>(TObject obj, Action<string> output)
		{
			_serializer(obj, output);
		}

		public TResult Deserialize<TResult>(string text)
		{
			return (TResult)(object)_deserializer(text);
		}

		public TypeReader<T> GetReader()
		{
			return _typeSerializer.GetReader();
		}

		public TypeWriter<T> GetWriter()
		{
			return _typeSerializer.GetWriter();
		}
    }
}
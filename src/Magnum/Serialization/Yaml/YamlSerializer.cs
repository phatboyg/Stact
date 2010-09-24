using System;
using System.IO;
using System.Text;


namespace Magnum.Serialization.Yaml
{
    public class YamlSerializer :
        YamlParser,
        Serializer
    {
        static readonly TypeSerializerCache _typeSerializerCache;

		[ThreadStatic]
		static YamlTypeSerializerCache _typeSerializers;

		static YamlSerializer()
		{
			_typeSerializerCache = new TypeSerializerCacheImpl();
		}

		public void Serialize<T>(T obj, TextWriter writer)
		{
			YamlTypeSerializer serializer = GetTypeSerializer(typeof(T));
            writer.Write(DocumentStartString);

			serializer.Serialize(obj, writer.Write);

            writer.Write(DocumentEndString);
		}

		public string Serialize<T>(T obj)
		{
			var sb = new StringBuilder(4096);
			using (var writer = new StringWriter(sb))
				Serialize(obj, writer);

			return sb.ToString();
		}

		public T Deserialize<T>(string text)
		{
			YamlTypeSerializer serializer = GetTypeSerializer(typeof(T));

			return serializer.Deserialize<T>(text);
		}

		public T Deserialize<T>(TextReader reader)
		{
			YamlTypeSerializer serializer = GetTypeSerializer(typeof(T));

			return serializer.Deserialize<T>(reader.ReadToEnd());
		}

		static YamlTypeSerializer GetTypeSerializer(Type type)
		{
			if (_typeSerializers == null)
				_typeSerializers = new YamlTypeSerializerCache(GetTypeSerializerCache());

			return _typeSerializers[type];
		}

		static TypeSerializerCache GetTypeSerializerCache()
		{
			return _typeSerializerCache;
		}
    }
}
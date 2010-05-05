namespace Magnum.Serialization
{
	using System;
	using System.Collections.Generic;
	using System.Xml;
	using Monads;

	public class ObjectSerializer<T> :
		TypeSerializer<T>
		where T : class
	{
	//	private readonly IEnumerable<SerializeObjectProperty<T>> _propertyCache;
		private readonly Type _type;

		public ObjectSerializer(Func<Type, TypeSerializer> getSerializer)
		{
		//	_propertyCache = new SerializeObjectPropertyCache<T>();

			_type = typeof(T);
		}

//		public IEnumerable<K<Action<XmlWriter>>> GetSerializationActions(ISerializerContext context, string localName, object value)
//		{
//			if (value == null)
//				yield break;
//
//			yield return output => output(writer =>
//			{
//				bool isDocumentElement = writer.WriteState == WriteState.Start;
//
//				writer.WriteStartElement(prefix, localName, _ns);
//
//				if (isDocumentElement)
//					context.WriteNamespaceInformationToXml(writer);
//			});
//
//			foreach (SerializeObjectProperty<T> property in _propertyCache)
//			{
//				object obj = property.GetValue((T)value);
//				if (obj == null)
//					continue;
//
//				var serializeType = context.MapType(typeof(T), property.PropertyType, obj);
//				IEnumerable<K<Action<XmlWriter>>> enumerable = context.SerializeObject(property.Name, serializeType, obj);
//				foreach (K<Action<XmlWriter>> action in enumerable)
//				{
//					yield return action;
//				}
//			}
//
//			yield return output => output(writer => { writer.WriteEndElement(); });
//		}

		public TypeReader<T> GetReader()
		{
			throw new NotImplementedException();
		}

		public TypeWriter<T> GetWriter()
		{
			return (value, output) =>
				{
					if(value == null)
						return;

					
				};
		}
	}
}
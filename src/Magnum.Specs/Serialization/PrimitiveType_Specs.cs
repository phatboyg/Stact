namespace Magnum.Specs.Serialization
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using System.Xml;
	using Magnum.Monads;
	using Magnum.Reflection;
	using NUnit.Framework;
	using TestSubjects;

	[TestFixture]
	public class Primitive_types_should_be_properly_serialized
	{
		private XmlObjectSerializer _serializer;

		[SetUp]
		public void Setup()
		{
			_serializer = new XmlObjectSerializer();
		}

		[Test]
		public void An_integer()
		{
			using ( var s = new MemoryStream())
			{
				int value = 47;

				_serializer.Serialize(value, s);

				s.Position = 0;

				Trace.WriteLine("XML: " + Encoding.UTF8.GetString(s.ToArray()));

				int actual = _serializer.Deserialize<int>(s);

				actual.ShouldEqual(value);
			}
		}

		[Test]
		public void A_class_containing_primitive_types_should_be_properly_serialized()
		{
			PrimitiveClass original = new PrimitiveClass
				{
					BoolValue = true,
					ByteValue = 221,
					CharValue = 'A',
					DecimalValue = 1234.56m,
					DoubleValue = 1234567.890,
					FloatValue = 123.456f,
					IntValue = 47,
					LongValue = 4123456789,
					ShortValue = 32123,
					StringValue = "Hello, World.",
				};

			using ( var s = new MemoryStream())
			{
				_serializer.Serialize(original, s);
				
				s.Position = 0;
				Trace.WriteLine("XML: " + Encoding.UTF8.GetString(s.ToArray()));

				var copy = _serializer.Deserialize<PrimitiveClass>(s);

				Assert.AreEqual(original, copy);

				Assert.AreNotSame(original, copy);
			}
		}



		[Test]
		public void Should_be_able_to_serialize_a_property()
		{
			PrimitiveClass original = new PrimitiveClass {IntValue = 47};

			PropertyInfo info = original.GetType().GetProperty("IntValue", BindingFlags.Instance | BindingFlags.Public);

			PropertySerializer serializer = PropertySerializer.Create(info);


			var settings = new XmlWriterSettings
				{
					Encoding = Encoding.UTF8,
					Indent = true,
				};

			string document = settings.GenerateXml(writer =>
				{
					writer.WriteStartDocument();
					writer.WriteStartElement("message");

					serializer.Serialize(original, writer);

				});

			Assert.AreEqual(string.Empty, document);
		}

		[Test]
		public void Should_be_able_to_parse_an_xml_message()
		{
			var settings = new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
			};

			byte[] data = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?><message intValue=\"47\"><subClass><stringValue>Hello</stringValue></subClass></message>");

			using(MemoryStream stream = new MemoryStream(data))
			{
				var parserExtreme = new XmlParserExtreme();

				parserExtreme.Parse<object>(stream, result =>
					{
						
					});
			}

			settings.ParseXml(data, reader =>
				{
					while(reader.Read())
					{
						Trace.WriteLine(reader.NodeType);
					}
					
				});
		}
	
	}









	public class XmlParserExtreme
	{
		private readonly Dictionary<XmlNodeType, NodeParser> _parsers = new Dictionary<XmlNodeType, NodeParser>();
	
		public XmlParserExtreme()
		{

			_parsers.Add(XmlNodeType.XmlDeclaration, new IgnoreParser());

			_parsers.Add(XmlNodeType.Element, new ElementParser());
			
		}

		public void Parse<T>(Stream stream, K<T> output)
		{
			var settings = new XmlReaderSettings()
			{
			};

			using (StreamReader streamReader = new StreamReader(stream))
			using (XmlReader reader = XmlReader.Create(streamReader, settings))
			{
				while (reader.Read())
				{
					NodeParser parser;
					if (_parsers.TryGetValue(reader.NodeType, out parser))
						parser.Parse(reader, output);
					else
					{
						// maybe try to build one? nah

						throw new InvalidOperationException("No parser to handle node of type: " + reader.NodeType);
					}

					if(reader.EOF)
					{
						Trace.WriteLine("And we are outta here.");
						
					}
				}
			}
		}
	}

	public class IgnoreParser :
		NodeParser
	{
		public void Parse<T>(XmlReader reader, K<T> output)
		{
			
		}
	}

	public class ElementParser :
		NodeParser
	{
		public void Parse<T>(XmlReader reader, K<T> output)
		{
			string name = reader.Name;
			string ns = reader.NamespaceURI;

			Trace.WriteLine("Reading element: " + ( ns == null ? ( ns + ":" ) : "") + name);

			while(reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
					return;

				Trace.WriteLine("Reading Node Type: " + reader.NodeType);
			}

		}
	}

	public interface NodeParser
	{
		void Parse<T>(XmlReader reader, K<T> output);
	}


	public static class CrazyShit
	{

		public static string ToCamelCase(this string value)
		{
			if (value == null)
				return null;

			if(value.Length==0)
				return string.Empty;

			if (value.Length == 1)
				return value.ToLower();

			return value.Substring(0, 1).ToLowerInvariant() + value.Substring(1);
		}

		public static string GenerateXml(this XmlWriterSettings settings, Action<XmlWriter> action)
		{
			using (var memoryStream = new MemoryStream())
			using (var streamWriter = new StreamWriter(memoryStream))
			using (var xmlWriter = XmlWriter.Create(streamWriter))
			{
				if (xmlWriter == null)
					throw new NullReferenceException("The XmlWriter could not be created");

				action(xmlWriter);

				xmlWriter.Flush();
				xmlWriter.Close();

				return settings.Encoding.GetString(memoryStream.ToArray());
			}
		}

		public static void ParseXml(this XmlWriterSettings settings, byte[] data, Action<XmlReader> action)
		{
			using (var memoryStream = new MemoryStream(data))
			using (var streamReader = new StreamReader(memoryStream))
			using (var xmlReader = XmlReader.Create(streamReader))
			{
				action(xmlReader);

				xmlReader.Close();
			}
		}
	}

	// TODO System.Security.SecurityElement.Escape()

	public class XmlObjectSerializer
	{
		ObjectWriterFactory _objectWriters = new ObjectWriterFactory();

		public string DocumentNamespace { get; set; }

		public XmlObjectSerializer()
		{
			DocumentNamespacePrefix = "mt";
			DocumentNamespace = "http://tempuri.org";
		}

		protected string DocumentNamespacePrefix { get; set; }

		public void Serialize(object value, MemoryStream stream)
		{
			using (var writer = XmlWriter.Create(stream, new XmlWriterSettings {Indent = true, Encoding = Encoding.UTF8}))
			{
				writer.WriteStartDocument();
				writer.WriteStartElement("message");
				writer.WriteAttributeString("xmlns", DocumentNamespacePrefix, null, DocumentNamespace);
				writer.WriteAttributeString("xmlns", DocumentNamespacePrefix, null, DocumentNamespace);


				foreach (var action in _objectWriters.For(value))
				{
					action(writer);
				}

				writer.WriteEndElement();
				writer.WriteEndDocument();
				writer.Flush();
				writer.Close();
			}
		}



		public T Deserialize<T>(MemoryStream stream)
		{
			using(var reader = XmlReader.Create(stream, new XmlReaderSettings{IgnoreWhitespace = true}))
			{
				

			}

			throw new NotImplementedException();
		}
	}

	public class ObjectWriterFactory
	{
		private IntegerXmlWriter _integerWriter = new IntegerXmlWriter();

		public IEnumerable<Action<XmlWriter>> For(object value)
		{
			if(value == null)
				yield break;

			if (value.GetType() == typeof(int))
				yield return writer => _integerWriter.WriteElement(writer, (int) value);

		}
	}

	public class PropertySerializer
	{
		private readonly PropertyInfo _info;
		private readonly FastProperty _property;
		private readonly IntegerStringWriter _writer;
		private readonly string _attributeName;
		private string _namespace;

		private PropertySerializer(PropertyInfo info, FastProperty property, IntegerStringWriter writer)
		{
			_info = info;
			_property = property;
			_writer = writer;

			_attributeName = info.Name.ToCamelCase();
			_namespace = string.Empty;
		}

		public static PropertySerializer Create(PropertyInfo info)
		{
			FastProperty accessor = new FastProperty(info);

			if(info.PropertyType == typeof(int))
			{
				IntegerStringWriter writer = new IntegerStringWriter();

				return new PropertySerializer(info, accessor, writer);
			}

			throw new NotSupportedException();
		}

		public void Serialize(object obj, XmlWriter writer)
		{
			object value = _property.Get(obj);

			_writer.GetWriter(value)(output =>
				{
					writer.WriteAttributeString(_attributeName, _namespace, output);
				});

		}
	}




	public interface StringWriter
	{
		
	}

	public interface StringReader
	{
		
	}

	public class IntegerStringWriter :
		StringReader,
		StringWriter
	{
		public K<string> GetWriter(object value)
		{
			return respond =>
				{
					if ( value == null)
						respond("0");
					else
					{
						respond(((int) value).ToString());
					}
				};
		}
	}



	public class IntegerXmlWriter
	{
		public void WriteElement(XmlWriter writer, int value)
		{
			writer.WriteStartElement(typeof(int).Name, typeof(int).Namespace);
			writer.WriteValue(value);
			writer.WriteEndElement();
		}
	}

	public interface ObjectXmlWriter
	{
		void Write(XmlWriter writer);
	}


	public class ClassParser
	{
		public static IEnumerator<T> FindClasses<T>(object k)
			where T : class
		{
			if (typeof(T).IsAssignableFrom(k.GetType()))
				yield return (k as T);

			var usedTypes = new HashSet<Type>();

			PropertyInfo[] properties = k.GetType().GetProperties();
			foreach (PropertyInfo info in properties)
			{
				if (usedTypes.Contains(info.PropertyType))
					continue;

				if (info.PropertyType.IsValueType)
					continue;

				if (typeof(T).IsAssignableFrom(info.PropertyType))
				{
					object child = info.GetValue(k, null);

					yield return child as T;
				}
			}
		}
	}
}
namespace Magnum.Serialization.Specs
{
	using System;
	using System.IO;
	using System.Text;
	using NUnit.Framework;
	using Machine.Specifications;

	[Concern("XML Serialization")]
	public class When_deserializing_an_object_from_xml
	{
		private const string _xml = @"<?xml version=""1.0"" encoding=""utf-8""?><object type=""Magnum.Serialization.Specs.SimpleClass""><CorrelationId>47e23391-de17-4612-a4bf-f907a72e4d64</CorrelationId><Name>Johnson</Name><Created>2008-10-01 01:02:03Z</Created></object>";

		private Establish context = () =>
			{
				_xmlBytes = new UTF8Encoding(false).GetBytes(_xml);
			};

		private Because of = () =>
			{
				using (MemoryStream buffer = new MemoryStream(_xmlBytes))
				{
					using (IObjectSerializer _serializer = new PropertySerializer())
					using (IObjectParser _formatter = new XmlObjectParser(buffer))
						_loaded = _serializer.Deserialize<SimpleClass>(_formatter);
				}
			};

		private It should_load_the_property_correlationid = () =>
			{
				Assert.AreEqual(new Guid("47e23391-de17-4612-a4bf-f907a72e4d64"), _loaded.CorrelationId);
			};

		private It should_load_the_property_name = () =>
			{
				Assert.AreEqual("Johnson", _loaded.Name);
			};

		private It should_load_the_property_created = () =>
			{
				Assert.AreEqual(new DateTime(2008,10,1,1,2,3, DateTimeKind.Utc), _loaded.Created.ToUniversalTime());
			};

		private static byte[] _xmlBytes;
		private static SimpleClass _loaded;
	}
}
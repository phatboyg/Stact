namespace Magnum.Serialization.Specs
{
	using System;
	using System.IO;
	using System.Text;
	using NUnit.Framework;
	using Machine.Specifications;

	[Concern("XML Serializer")]
	public class When_serializing_an_object_to_xml
	{
		private Establish context = () =>
			{
				_data = new SimpleClass("Johnson");

			};

		private Because of = () =>
			{
				using(MemoryStream buffer = new MemoryStream())
				{
					using (IObjectSerializer _serializer = new PropertySerializer())
					{
						using (IObjectFormatter _formatter = new XmlObjectFormatter(buffer))
							_serializer.Serialize(_formatter, _data);

						_xml = Encoding.UTF8.GetString(buffer.ToArray());
					}
				}
			};

		private It should_create_valid_xml = () =>
			{
				Assert.AreEqual(_expectedXml, _xml);
			};


		private static SimpleClass _data;
		private static string _xml;
		private const string _expectedXml = @"<?xml version=""1.0"" encoding=""utf-8""?><object type=""Magnum.Serialization.Specs.SimpleClass""><CorrelationId>47e23391-de17-4612-a4bf-f907a72e4d64</CorrelationId><Name>Johnson</Name><Created>2008-10-01 01:02:03Z</Created></object>";
	}

	[Serializable]
	public class SimpleClass
	{
		public SimpleClass(string name)
		{
			CorrelationId = new Guid("47E23391-DE17-4612-A4BF-F907A72E4D64");
			Name = name;
			Created = new DateTime(2008, 10, 1, 1, 2, 3, DateTimeKind.Utc);
		}

		public SimpleClass()
		{
		}

		public Guid CorrelationId { get;  set; }
		public string Name { get;  set; }
		public DateTime Created { get;  set; }
	}
}
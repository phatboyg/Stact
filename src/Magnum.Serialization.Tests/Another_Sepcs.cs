namespace Magnum.Serialization.Tests
{
	using System;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using System.Xml;
	using NUnit.Framework;

	[TestFixture]
	public class Another_Sepcs
	{
		[Test]
		public void How_the_fuck_is_this_hard()
		{
			FlatObject flat = new FlatObject("Some String", 47, 9.95m);

			Cerealizer cereal = new Cerealizer();

			string result = cereal.MilkIt(flat);

			Assert.AreEqual("Some XML", result);
		}


		public class FlatObject
		{
			private readonly string _text;
			private readonly int _number;
			private readonly decimal _amount;

			public FlatObject(string text, int number, decimal amount)
			{
				_text = text;
				_number = number;
				_amount = amount;
			}

			public string Text
			{
				get { return _text; }
			}

			public int Number
			{
				get { return _number; }
			}

			public decimal Amount
			{
				get { return _amount; }
			}
		}
	}

	public class Cerealizer
	{
		private readonly ISerializationFormatter _formatter = new XmlFormatter();

		public string MilkIt<T>(T flat)
		{
			Type objectType = typeof (T);

			_formatter.StartObject(objectType);

			PropertyInfo[] properties = objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

			foreach (PropertyInfo propertyInfo in properties)
			{
				Debug.WriteLine(string.Format("Property: {0}", propertyInfo.Name));

				object value = propertyInfo.GetValue(flat, BindingFlags.Instance | BindingFlags.Public, null, null, CultureInfo.InvariantCulture);

				_formatter.SetProperty(propertyInfo.Name, propertyInfo.PropertyType, value);

			}


			string result =  _formatter.GetString();

			Debug.WriteLine(result);
			return result;
		}
	}

	internal class XmlFormatter : ISerializationFormatter
	{
		private readonly XmlWriter _writer;
		private readonly MemoryStream _stream;

		public XmlFormatter()
		{
			_stream = new MemoryStream();
			_writer = XmlWriter.Create(_stream);

			_writer.WriteStartDocument();
		}

		public void StartObject(Type objectType)
		{
			_writer.WriteStartElement(objectType.Name);
			_writer.WriteAttributeString("type", objectType.FullName);
		}

		public string GetString()
		{
			_writer.WriteEndDocument();
			_writer.Flush();

			return Encoding.UTF8.GetString(_stream.ToArray());
		}

		public void SetProperty(string name, Type type, object value)
		{
			_writer.WriteStartElement(name);
			//_writer.WriteAttributeString("type", type.FullName);
			_writer.WriteValue(value);
			_writer.WriteEndElement();
		}
	}

	public interface ISerializationFormatter
	{
		void StartObject(Type objectType);
		string GetString();
		void SetProperty(string name, Type type, object value);
	}
}
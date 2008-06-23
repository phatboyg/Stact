namespace Magnum.Serialization.Tests
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Xml;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class Deserializer_Specs
	{
		[Test]
		public void Gett_The_Fuck_out()
		{
			string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><FlatObject type=\"Magnum.Serialization.Tests.Another_Sepcs+FlatObject\"><Text>Some String</Text><Number>47</Number><Amount>9.95</Amount></FlatObject>";

			StringReader reader = new StringReader(xml);

			Decereal tool = new Decereal();

			IMaker maker = new Maker();

			tool.Undo(reader, maker);

			object result = maker[0];

			Assert.That(result, Is.TypeOf(typeof (Another_Sepcs.FlatObject)));

			Another_Sepcs.FlatObject obj = result as Another_Sepcs.FlatObject;

			Assert.That(obj.Text, Is.EqualTo("Some String"));
			Assert.That(obj.Number, Is.EqualTo(47));
			Assert.That(obj.Amount, Is.EqualTo(9.95m));

		}
	}

	public class Maker : IMaker
	{
		private readonly List<object> _objects = new List<object>();

		public int CreateObject(Type t)
		{
			object o = FormatterServices.GetSafeUninitializedObject(t);
			_objects.Add(o);

			return _objects.Count - 1;
		}

		public void SetProperty(int objectId, string name, string value)
		{
			Debug.WriteLine("Trying to set property " + name + " to " + value);

			object o = _objects[objectId];
			Type t = o.GetType();

			PropertyInfo pi = t.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
			if (pi != null)
			{
				MethodInfo setter = pi.GetSetMethod(true);
				if (setter != null)
				{
					object valueObj = TypeDescriptor.GetConverter(pi.PropertyType).ConvertFromInvariantString(value);

					setter.Invoke(o, new object[] { valueObj });
					return;
				}
			}

			string fieldName = "_" + name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
			FieldInfo fi = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fi != null)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(fi.FieldType);
				if (converter.IsValid(value))
				{
					object valueObj = converter.ConvertFromInvariantString(value);
					fi.SetValue(o, valueObj);
				}
			}
			else
			{
				throw new ArgumentException("Unknown property value: " + name);
			}
		}

		public object this[int index]
		{
			get
			{
				return _objects[index];
			}
		}
	}

	internal class Decereal
	{
		public object Undo(TextReader xml, IMaker output)
		{
			object obj = null;

			using (XmlReader reader = XmlReader.Create(xml))
			{
				int currentObjectId = -1;

				while (reader.Read())
				{
					if (reader.IsStartElement())
					{
						string elementName = reader.Name;

						if (reader.HasAttributes)
						{
							reader.MoveToFirstAttribute();
							if (reader.Name == "type")
							{
								string objectType = reader.Value;
								Debug.WriteLine("Element Type: " + objectType);

								Type t = Type.GetType(objectType);

								currentObjectId = output.CreateObject(t);

								reader.MoveToContent();
							}
						}
						else
						{
							// no attributes, probably a property

							reader.MoveToContent();
							reader.Read();

							output.SetProperty(currentObjectId, elementName, reader.Value);
						}
					}
				}
			}

			return obj;
		}
	}

	public interface IMaker
	{
		int CreateObject(Type t);
		void SetProperty(int objectId, string name, string value);

		object this[int index] { get; }
	}
}
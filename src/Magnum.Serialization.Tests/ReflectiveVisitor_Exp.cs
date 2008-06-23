namespace Magnum.Serialization.Tests
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Xml;
	using NUnit.Framework;

	[TestFixture]
	public class ReflectiveVisitor_Exp
	{
		[Test]
		public void We_should_go_through_the_list_quickly()
		{
			DerivedObject obj = new DerivedObject("Chris", "Patterson", "Pimp");

			IGetTheFuckOut fomatter = new XmlOutputFucker();

			ISuperSerializer serializer = new ReflexivePimpDaddy(fomatter);



			serializer.Serialize(obj);


			Debug.WriteLine("XML: " + fomatter.GetString());

		}
	}

	public class ReflexivePimpDaddy : ISuperSerializer
	{
		private readonly IGetTheFuckOut _fomatter;

		public ReflexivePimpDaddy(IGetTheFuckOut fomatter)
		{
			_fomatter = fomatter;
		}

		public void Serialize<T>(T obj) where T : class
		{
			_fomatter.Start();

			SerializeObject(obj);



			_fomatter.Stop();
		}

		private void SerializeObject<T>(T obj) where T : class
		{
			Type objType = typeof (T);

			_fomatter.StartObject(objType);

			MemberInfo[] memberInfo = FormatterServices.GetSerializableMembers(objType);

			
			object[] objData = FormatterServices.GetObjectData(obj, memberInfo);

			for (int index = 0; index < memberInfo.Length; index++)
			{
				if(memberInfo[index].MemberType == MemberTypes.Field)
				{
					FieldInfo fieldInfo = (FieldInfo) memberInfo[index];

					Type fieldType = fieldInfo.FieldType;
					if(fieldType.IsValueType)
					{
						SerializeValueType(fieldInfo, objData[index]);
					}
					else
					{
						SerializeReference(fieldInfo, objData[index]);
					}
				}
			}


			_fomatter.EndObject(objType);
			
		}

		private void SerializeReference(FieldInfo fieldInfo, object obj)
		{
			if(fieldInfo.FieldType == typeof(string))
			{
				_fomatter.WriteString(fieldInfo, obj as string);
			}
			
		}

		private void SerializeValueType(FieldInfo fieldInfo, object obj)
		{
			_fomatter.WriteField("string", obj.ToString());
		}
	}

	public interface IObjectSerializer
	{
	}

	public class XmlOutputFucker : IGetTheFuckOut
	{
		private readonly MemoryStream _stream;
		private readonly XmlWriter _writer;

		public XmlOutputFucker()
		{
			 _stream = new MemoryStream();
			_writer = XmlWriter.Create(_stream);
		}

		public string GetString()
		{
			_writer.Flush();

			return Encoding.UTF8.GetString(_stream.ToArray());
		}

		public void Start()
		{
			_writer.WriteStartDocument();
		}

		public void Stop()
		{
			_writer.WriteEndDocument();
		}

		public void StartObject(Type type)
		{
			_writer.WriteStartElement("object");
			_writer.WriteAttributeString("type", type.FullName);
		}

		public void EndObject(Type type)
		{
			_writer.WriteEndElement();
		}

		public void WriteField(string name, string value)
		{
			_writer.WriteElementString(name, value);
			
		}

		public void WriteString(FieldInfo info, string value)
		{
			_writer.WriteElementString("string", value);
			
		}

		public void Dispose()
		{
			if(_stream != null)
			{
				_stream.Dispose();
			}
		}
	}

	public interface ISuperSerializer
	{
		void Serialize<T>(T obj) where T : class;
	}

	public interface IGetTheFuckOut : IDisposable
	{
		string GetString();
		void Start();
		void Stop();
		void StartObject(Type type);
		void EndObject(Type type);
		void WriteField(string name, string value);
		void WriteString(FieldInfo info, string value);
	}
}
namespace Magnum.Serialization
{
	using System;
	using System.Reflection;
	using System.Runtime.Serialization;

	public class BasicObjectSerializer : IObjectSerializer
	{
		private readonly IObjectFormatter _fomatter;

		public BasicObjectSerializer(IObjectFormatter fomatter)
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
				if (memberInfo[index].MemberType == MemberTypes.Field)
				{
					FieldInfo fieldInfo = (FieldInfo) memberInfo[index];

					Type fieldType = fieldInfo.FieldType;
					if (fieldType.IsValueType)
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
			if (fieldInfo.FieldType == typeof (string))
			{
				_fomatter.WriteString(fieldInfo, obj as string);
			}
		}

		private void SerializeValueType(FieldInfo fieldInfo, object obj)
		{
			_fomatter.WriteField(fieldInfo, obj.ToString());
		}

		public void Dispose()
		{

		}
	}
}
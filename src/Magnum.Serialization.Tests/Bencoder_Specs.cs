namespace Magnum.Serialization.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Runtime.Serialization;
	using System.Text;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class BEncoder_Specs
	{
		[Test]
		public void A_custom_built_serializer_should_work_too()
		{
			Type classType = typeof (DerivedObject);

			BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

			PropertyInfo[] properties = classType.GetProperties(flags);

			foreach (PropertyInfo info in properties)
			{
				Debug.WriteLine("Property: " + info.Name);
			}

			FieldInfo[] fields = classType.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.NonPublic);

			foreach (FieldInfo fieldInfo in fields)
			{
				Debug.WriteLine("Field: " + fieldInfo.Name);

//				DynamicMethod method = new DynamicMethod("Boing", fieldInfo.FieldType, null, GetType());
//				ILGenerator il = method.GetILGenerator();
				//
				//				LocalBuilder thisLocal = il.DeclareLocal(classType);
				//				il.Emit(OpCodes.Ldarg_0);
				//				EmitCastToReference(il, mappedType);
				//				il.Emit(OpCodes.Stloc, thisLocal.LocalIndex);
			}
		}

		[Test]
		public void A_list_of_values_should_be_formatted_properly()
		{
			string[] names = new string[] {"One", "Two", "Three"};

			BEncoder encoder = new BEncoder();

			encoder.Append(names);

			Assert.That(encoder.GetString(), Is.EqualTo("l3:One3:Two5:Threee"));
		}

		[Test]
		public void A_numeric_value_should_be_properly_formatted()
		{
			BEncoder encoder = new BEncoder();

			int value = 27;

			encoder.Append(value);

			Assert.That(encoder.GetString(), Is.EqualTo("i27e"));
		}

		[Test]
		public void Derived_objects_should_include_everything_needed()
		{
			DerivedObject ob = new DerivedObject("Joe", "Blow", "President");
		}


		[Test]
		public void Resurrecting_an_object_should_not_call_the_constructor()
		{
			DerivedObject obj = new DerivedObject("Joe", "Blow", "King");
			Assert.That(DerivedObject.Hits, Is.EqualTo(1));

			DerivedObject obj2 = new DerivedObject("Chris", "Patterson", "Pimp", obj);
			Assert.That(DerivedObject.Hits, Is.EqualTo(2));

			Type objectType = obj2.GetType();

			MemberInfo[] memberInfo = FormatterServices.GetSerializableMembers(objectType);

			object[] objectData = FormatterServices.GetObjectData(obj2, memberInfo);


			Assert.That(objectData.Length, Is.EqualTo(4));

			object newObj = FormatterServices.GetUninitializedObject(objectType);


			FormatterServices.PopulateObjectMembers(newObj, memberInfo, objectData);

			Assert.That(obj2, Is.EqualTo(newObj));
			Assert.That(DerivedObject.Hits, Is.EqualTo(2));
		}


		[Test]
		public void Lets_start_simple()
		{
			BEncoder encoder = new BEncoder();

			SimpleObject obj = new SimpleObject("Christopher", "Patterson");

			Type objType = obj.GetType();

			MemberInfo[] memberInfo = FormatterServices.GetSerializableMembers(objType);

			object[] objData = FormatterServices.GetObjectData(obj, memberInfo);

			for (int index = 0; index < memberInfo.Length; index++)
			{
				if (objData[index] == null)
				{
					encoder.AppendNull();
				}
				else
				{
					Type objectType = objData[index].GetType();

					if (objectType.IsValueType)
					{
						if(objectType == typeof(int))
						{
							Debug.WriteLine("Found integer: " + memberInfo[index].Name);

							encoder.Append((int)objData[index]);
						}
					}
					else
					{
						if (objectType == typeof (string))
						{
							Debug.WriteLine("Found String: " + memberInfo[index].Name);

							encoder.Append((string) objData[index]);
						}
					}
				}
			}

			Assert.That(encoder.GetString(), Is.EqualTo("11:Christopher9:Pattersonni37e"));
		}


		[Test]
		public void An_instance_of_the_object_should_be_creatable()
		{
			Type classType = typeof (SimpleObject);

			SerializationOptimizer optimizer = new SerializationOptimizerBuilder(classType);

			object obj = optimizer.CreateInstance();

			Assert.That(obj, Is.TypeOf(classType));
		}

		[Test]
		public void An_object_should_be_formatted_well()
		{
			SimpleObject obj = new SimpleObject("Joe", "Smith");

			BEncoder encoder = new BEncoder();

			encoder.Append(obj.FirstName);
			encoder.Append(obj.LastName);

			Assert.That(encoder.GetString(), Is.EqualTo("3:Joe5:Smith"));
		}


		[Test]
		public void Parse_that_string()
		{
			string simple = "15:Chris Patterson";

			byte[] bytes = Encoding.UTF8.GetBytes(simple);

			MemoryStream mstream = new MemoryStream(bytes);


			EncoderState state = EncoderState.None;

			List<object> objectDataList = new List<object>();

			int length = 0;

			byte[] buffer = null;
			int offset = 0;

			try
			{
				for (int index = 0; index < mstream.Length; index++ )
				{
					int b = mstream.ReadByte();
					switch (state)
					{
						case EncoderState.None:
							if (b >= '0' && b <= '9')
							{
								// we have a block of data, let's read it
								state = EncoderState.InLength;

								length = b - '0';
							}
							break;

						case EncoderState.InLength:
							if (b >= '0' && b <= '9')
							{
								length = length*10 + (b - '0');
							}
							else if (b == ':')
							{
								state = EncoderState.InData;
								offset = 0;
								buffer = new byte[length];
							}
							break;

						case EncoderState.InData:
							buffer[offset++] = (byte) b;
							if (offset == length)
							{
								objectDataList.Add(Encoding.UTF8.GetString(buffer));

								state = EncoderState.None;
							}
							break;

						default:
							throw new ApplicationException("Ouch!");
					}
				}
			}
			catch (Exception ex)
			{
				
			}

			Assert.That(objectDataList.Count, Is.EqualTo(1));
		}


		internal enum EncoderState
		{
			None,
			InLength,
			InData
		}








		public static Type GetMemberUnderlyingType(MemberInfo member)
		{

			switch (member.MemberType)
			{
				case MemberTypes.Field:
					return ((FieldInfo)member).FieldType;
				case MemberTypes.Property:
					return ((PropertyInfo)member).PropertyType;
				case MemberTypes.Event:
					return ((EventInfo)member).EventHandlerType;
				default:
					throw new ArgumentException("MemberInfo must be if type FieldInfo, PropertyInfo or EventInfo", "member");
			}
		}




		[Test]
		public void SampleBuild()
		{
			BEncoder encoder = new BEncoder();

			string name = "Chris Patterson";

			encoder.Append(name);

			Assert.That(encoder.GetString(), Is.EqualTo("15:Chris Patterson"));
		}


		//		private static void GetReflectionOptimizer(Type type)
		//		{
		//				FieldInfo[] fields = FlattenInheritanceHierarchy(type);
		//				IGetter[] getters =
		//				  new IGetter[fields.Length];
		//				ISetter[] setters =
		//				  new ISetter[fields.Length];
		//				for (int i = 0; i < fields.Length; i++)
		//				{
		//					getters[i] = new FieldGetter(
		//					   fields[i],
		//					   type,
		//					   fields[i].Name);
		//					setters[i] = new FieldSetter(
		//					   fields[i],
		//					   type,
		//					   fields[i].Name);
		//				}
		//				BytecodeProviderImpl bytecodeProvider = new BytecodeProviderImpl();
		//				reflectionOptimizers.Add(
		//					type,
		//					bytecodeProvider.GetReflectionOptimizer(
		//					   type,
		//					   getters,
		//					   setters)
		//				);
		//			}
		//			return reflectionOptimizers[type];
		//		}
	}


	public interface ISerial
	{
		void Serialize(object obj);
	}

	[Serializable]
	public class SimpleObject : IEquatable<SimpleObject>
	{
		private readonly string _firstName;
		private readonly string _lastName;
		private string _unused;
		private int _otherwise = 37;

		public SimpleObject(string firstName, string lastName)
		{
			_firstName = firstName;
			_lastName = lastName;
		}

		public string LastName
		{
			get { return _lastName; }
		}

		public string FirstName
		{
			get { return _firstName; }
		}

		public bool Equals(SimpleObject simpleObject)
		{
			if (simpleObject == null) return false;
			return Equals(_firstName, simpleObject._firstName) && Equals(_lastName, simpleObject._lastName);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as SimpleObject);
		}

		public override int GetHashCode()
		{
			return (_firstName != null ? _firstName.GetHashCode() : 0) + 29*(_lastName != null ? _lastName.GetHashCode() : 0);
		}
	}

	[Serializable]
	public class DerivedObject : SimpleObject, IEquatable<DerivedObject>
	{
		private readonly string _title;
		//private readonly SimpleObject _child;

		public static int Hits
		{
			get { return _hits; }
		}

		private static int _hits = 0;

		public DerivedObject(string firstName, string lastName, string title) : base(firstName, lastName)
		{
			_hits++;
			_title = title;
		}

		public DerivedObject(string firstName, string lastName, string title, SimpleObject child) : base(firstName, lastName)
		{
			_hits++;
			_title = title;
		//	_child = child;
		}

		public string Title
		{
			get { return _title; }
		}

		//public SimpleObject Child
	//	{
		//	get { return _child; }
		//}

		public bool Equals(DerivedObject derivedObject)
		{
			if (derivedObject == null) return false;
			if (Equals(_title, derivedObject._title) == false)
				return false;
		//	if (Equals(_child, derivedObject._child) == false)
		//		return false;

			return base.Equals(derivedObject);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as DerivedObject);
		}

		public override int GetHashCode()
		{
			return _title != null ? _title.GetHashCode() : 0;
		}
	}
}
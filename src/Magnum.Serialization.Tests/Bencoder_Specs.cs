namespace Magnum.Serialization.Tests
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class BEncoder_Specs
	{
		[Test]
		public void A_custom_built_serializer_should_work_too()
		{
			Type classType = typeof (SimpleObject);

			FieldInfo[] fields = classType.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.NonPublic);

			foreach (FieldInfo fieldInfo in fields)
			{
				DynamicMethod method = new DynamicMethod("Boing", fieldInfo.FieldType, null, GetType());
				ILGenerator il = method.GetILGenerator();
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


	public class SimpleObject
	{
		private readonly string _firstName;
		private readonly string _lastName;

		private SimpleObject()
		{
		}

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
	}
}
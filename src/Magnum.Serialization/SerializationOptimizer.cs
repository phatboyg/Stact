namespace Magnum.Serialization
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;

	public delegate object CreateInstanceInvoker();

	public interface ISerializationOptimizer
	{
		object CreateInstance();
	}

	public class SerializationOptimizer :
		ISerializationOptimizer
	{
		private readonly CreateInstanceInvoker _createInstance;

		public SerializationOptimizer(CreateInstanceInvoker createInstance)
		{
			_createInstance = createInstance;
		}

		public object CreateInstance()
		{
			return _createInstance();
		}
	}

	public class SerializationOptimizerBuilder
	{
		private static readonly BindingFlags AnyVisibilityInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		private static readonly Type[] NoClasses = Type.EmptyTypes;

		private readonly CreateInstanceInvoker _createInstance;

		public SerializationOptimizerBuilder(Type type)
		{
			_createInstance = CreateCreateInstanceMethod(type);
		}

		public static implicit operator SerializationOptimizer(SerializationOptimizerBuilder builder)
		{
			return new SerializationOptimizer(builder._createInstance);
		}

		private static CreateInstanceInvoker CreateCreateInstanceMethod(Type type)
		{
			if (type.IsInterface || type.IsAbstract)
				return null;

			DynamicMethod method = new DynamicMethod(string.Empty, typeof (object), null, type, true);

			ILGenerator il = method.GetILGenerator();

			if (type.IsValueType)
			{
				LocalBuilder tmpLocal = il.DeclareLocal(type);
				il.Emit(OpCodes.Ldloca, tmpLocal);
				il.Emit(OpCodes.Initobj, type);
				il.Emit(OpCodes.Ldloc, tmpLocal);
				il.Emit(OpCodes.Box, type);
			}
			else
			{
				ConstructorInfo constructor = type.GetConstructor(AnyVisibilityInstance, null, CallingConventions.HasThis, NoClasses, null);
				if (constructor == null)
					throw new ApplicationException("Object class " + type + " must declare a default (no-argument) constructor");

				il.Emit(OpCodes.Newobj, constructor);
			}

			il.Emit(OpCodes.Ret);

			return (CreateInstanceInvoker) method.CreateDelegate(typeof (CreateInstanceInvoker));
		}
	}
}
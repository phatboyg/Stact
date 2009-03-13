namespace Magnum.Experiments.Reflection
{
	using System;
	using System.Collections.Generic;

	public static class TypeExtensions
	{
		public static IEnumerable<Type> GetDeclaredTypesForGenericInterfaces(this Type type)
		{
			var interfaces = type.GetInterfaces();
			foreach (var item in interfaces)
			{
				if (!item.IsGenericType)
					continue;

				var arguments = item.GetGenericArguments();
				foreach (var argumentType in arguments)
				{
					yield return argumentType;
				}
			}

			var baseType = type.BaseType;
			if (baseType != null)
			{
				foreach (Type baseGenericType in baseType.GetDeclaredTypesForGenericInterfaces())
				{
					yield return baseGenericType;
				}
			}
		}

		public static IEnumerable<Type> GetDeclaredTypesForGenericType(this Type type)
		{
			if (type.IsGenericType)
			{
				var arguments = type.GetGenericArguments();
				foreach (var argumentType in arguments)
				{
					yield return argumentType;
				}
			}

			var baseType = type.BaseType;
			if (baseType != null)
			{
				foreach (Type baseGenericType in baseType.GetDeclaredTypesForGenericType())
				{
					yield return baseGenericType;
				}
			}
		}
	}
}
namespace Magnum.Experiments.Reflection
{
	using System;
	using System.Collections.Generic;

	public static class InterfaceExtensions
	{

		public static IEnumerable<Type> GetDeclaredTypesForGeneric(this Type type, Type interfaceType)
		{
			foreach (var generic in type.GetGenericInterfacesFor(interfaceType))
			{
				foreach (var arg in generic.GetGenericArguments())
				{
					yield return arg;
				}
			}
		}

		public static IEnumerable<Type> GetGenericInterfacesFor(this Type type, Type interfaceType)
		{
			var candidates = type.GetInterfaces();
			foreach (var candidate in candidates)
			{
				if (!candidate.IsGenericType)
				{
					continue;
				}

				var definition = candidate.GetGenericTypeDefinition();
				if (definition == interfaceType)
				{
					yield return candidate;
				}
			}


		}

		
	}
}
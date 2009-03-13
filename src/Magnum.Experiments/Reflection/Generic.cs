namespace Magnum.Experiments.Reflection
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	public class Generic
	{
		public static void Call<T>(T instance, Expression<Action<T>> method, object arg0)
		{
			MethodCallExpression methodCall = method.Body as MethodCallExpression;
			if (methodCall == null)
				throw new InvalidOperationException("The expression specified is not supported");

			CallMethod(instance, methodCall, arg0);
		}

		private static void CallMethod<T>(T instance, MethodCallExpression methodCall, object arg0)
		{
			if (!methodCall.Method.IsGenericMethod)
				throw new InvalidOperationException("Support for non-generic methods has not yet been implemented.");

			CallGenericMethod(instance, methodCall, arg0);
		}

		private static void CallGenericMethod<T>(T instance, MethodCallExpression methodCall, object arg0)
		{
			if(arg0 == null)
				throw new ArgumentNullException("arg0", "The generic argument cannot be null");

			var instanceType = typeof (T);
			var argType = arg0.GetType();
			Type[] genericTypes = argType.GetDeclaredTypesForGenericInterfaces().ToArray();

			foreach (var type in genericTypes)
			{
				Trace.WriteLine("Found Type: " + type.FullName);
			}

			MethodInfo genericMethodDefinition = methodCall.Method.GetGenericMethodDefinition();

			var genericMethodArguments = genericMethodDefinition.GetGenericArguments();
			foreach (var type in genericMethodArguments)
			{
				Trace.WriteLine("Generic Argument Required: " + type.Name);
			}

			var methodArguments = genericMethodDefinition.GetParameters();
			foreach (ParameterInfo info in methodArguments)
			{
				Trace.WriteLine("Method Parameters = " + info.Name);
				Trace.WriteLine(string.Format("Parameter is Generic: {0}", info.ParameterType.IsGenericType ? "Yes" : "No"));
				var parameterGenericArguments = info.ParameterType.GetDeclaredTypesForGenericType();
				foreach (Type type in parameterGenericArguments)
				{
					Trace.WriteLine("Parameter Has Generic Type: " + type.Name);
				}
			}

			MethodInfo methodInfo = genericMethodDefinition.MakeGenericMethod(genericTypes);

			var instanceParameter = Expression.Parameter(typeof(object), "instance");
			var inputParameter = Expression.Parameter(typeof(object), "input");
			ParameterExpression[] arguments = { instanceParameter, inputParameter };

			var instanceCast = Expression.TypeAs(instanceParameter, instanceType);
			var valueCast = argType.IsValueType ? Expression.Convert(inputParameter, argType) : Expression.TypeAs(inputParameter, argType);

			var call = Expression.Call(instanceCast, methodInfo, valueCast);

			var callMethod = Expression.Lambda<Action<object, object>>(call, arguments).Compile();

			callMethod(instance, arg0);
		}
	}
}
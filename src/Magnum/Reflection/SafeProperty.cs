// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Magnum.Reflection
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;
	using Extensions;

	/// <summary>
	/// Use to safely set a property on an object, including the expansion of any lists as necessary
	/// and creation of reference properties. May not be suitable to all situations, but works great
	/// for deserializing data into an empty object graph.
	/// </summary>
	/// <typeparam name="T">The type of object being targeted.</typeparam>
	public class SafeProperty<T>
	{
		private readonly Action<T, object> _setter;

		public SafeProperty(Expression<Func<T, object>> expression)
		{
			_setter = CreateSetter(expression);
		}

		public Action<T, object> Setter
		{
			get { return _setter; }
		}

		public void Set(T obj, object value)
		{
			_setter(obj, value);
		}

		private static Action<T, object> CreateSetter(Expression<Func<T, object>> expression)
		{
			MemberExpression me = expression.GetMemberExpression();

			if (me.Member.MemberType == MemberTypes.Property)
			{
				var property = (PropertyInfo) me.Member;

				if (me.Expression.NodeType == ExpressionType.Parameter && me.Expression.Type == typeof (T))
				{
					return FastProperty<T>.GetSetMethod(property, true);
				}

				// we have a nested expression, we need to dig it out
				if (me.Expression.NodeType == ExpressionType.MemberAccess)
				{
					return CreateNullSafeSetter(property, (MemberExpression) me.Expression, expression.Parameters[0]);
				}

				if (me.Expression.NodeType == ExpressionType.Call)
				{
					return CreateNullSafeListSetter(property, (MethodCallExpression) me.Expression, expression.Parameters[0]);
				}
			}

			throw new NotImplementedException("Not ready yet.");
		}

		private static Action<T, object> CreateNullSafeSetter(PropertyInfo propertyInfo,
		                                                      MemberExpression me,
		                                                      ParameterExpression parameter)
		{
			Func<T, object> getDeclaring = CreateNullSafeGetter(me, parameter);

			Action<object, object> setProperty = FastProperty.GetSetMethod(propertyInfo, true);

			return (o, v) =>
				{
					object declared = getDeclaring(o);

					setProperty(declared, v);
				};
		}

		private static Action<T, object> CreateNullSafeListSetter(PropertyInfo propertyInfo,
		                                                          MethodCallExpression mce,
		                                                          ParameterExpression parameter)
		{
			if (mce.Object.NodeType == ExpressionType.MemberAccess)
			{
				var me = mce.Object as MemberExpression;
				Func<T, object> getDeclaring = CreateNullSafeGetter(me, parameter);

				int getIndex = 0;
				if (mce.Arguments[0].NodeType == ExpressionType.Constant)
					getIndex = (int) (((ConstantExpression) mce.Arguments[0]).Value);

				Func<T, object> getTarget = Expression.Lambda<Func<T, object>>(mce, parameter).Compile();

				Action<object, object> setProperty = FastProperty.GetSetMethod(propertyInfo, true);

				Func<object, int> getCount = GetCountMethod(mce);
				Func<object> objectFactory = GetObjectFactory(mce.Method.ReturnType);
				Action<object, object> addMethod = GetAddMethod(mce);

				return (o, v) =>
					{
						object declared = getDeclaring(o);

						int count = getCount(declared);
						for (int index = count; index <= getIndex; index++)
						{
							addMethod(declared, objectFactory());
						}

						object target = getTarget(o);

						setProperty(target, v);
					};
			}

			throw new NotImplementedException("Not implementing this yet");
		}

		private static Func<object, int> GetCountMethod(MethodCallExpression expression)
		{
			if (expression.Arguments.Count == 1 && expression.Arguments[0].Type == typeof (int))
			{
				Type interfaceType = expression.Object.Type;
				Type argument = interfaceType.GetGenericArguments()[0];
				Type callType = typeof (ICollection<>).MakeGenericType(argument);

				MethodInfo getMethod = callType.GetProperty("Count").GetGetMethod();

				ParameterExpression input = Expression.Parameter(typeof (object), "input");
				UnaryExpression cast = Expression.TypeAs(input, expression.Object.Type);

				return Expression.Lambda<Func<object, int>>(Expression.Call(cast, getMethod), input).Compile();
			}

			throw new NotImplementedException("No idea why this won't work");
		}

		private static Action<object, object> GetAddMethod(MethodCallExpression expression)
		{
			if (expression.Arguments.Count == 1 && expression.Arguments[0].Type == typeof (int))
			{
				Type interfaceType = expression.Object.Type;
				Type argument = interfaceType.GetGenericArguments()[0];
				Type callType = typeof (ICollection<>).MakeGenericType(argument);

				MethodInfo getMethod = callType.GetMethod("Add");

				ParameterExpression input = Expression.Parameter(typeof (object), "input");
				ParameterExpression value = Expression.Parameter(typeof (object), "value");
				UnaryExpression cast = Expression.TypeAs(input, expression.Object.Type);
				UnaryExpression castValue = Expression.TypeAs(value, argument);

				return
					Expression.Lambda<Action<object, object>>(Expression.Call(cast, getMethod, castValue), input, value).Compile();
			}

			throw new NotImplementedException("No idea why this won't work");
		}

		private static Func<T, object> CreateNullSafeGetter(MemberExpression me,
		                                                    ParameterExpression parameter)
		{
			if (me.Member.MemberType == MemberTypes.Property)
			{
				var property = (PropertyInfo) me.Member;

				if (me.Expression.NodeType == ExpressionType.Parameter && me.Expression == parameter)
				{
					Func<T, object> getDeclaring = FastProperty<T>.GetGetMethod(property);
					Action<T, object> setDeclaring = FastProperty<T>.GetSetMethod(property, true);
					Func<object> objectFactory = GetObjectFactory(property.PropertyType);

					return (o) =>
						{
							object declared = getDeclaring(o);
							if (declared == null)
							{
								declared = objectFactory();
								setDeclaring(o, declared);
							}

							return declared;
						};
				}

				if (me.Expression.NodeType == ExpressionType.MemberAccess)
				{
					Func<T, object> getChild = CreateNullSafeGetter((MemberExpression) me.Expression, parameter);

					Func<object, object> getDeclaring = FastProperty.GetGetMethod(property);
					Action<object, object> setDeclaring = FastProperty.GetSetMethod(property, true);

					Func<object> objectFactory = GetObjectFactory(property.PropertyType);

					return (o) =>
						{
							object target = getChild(o);

							object declared = getDeclaring(target);
							if (declared == null)
							{
								declared = objectFactory();
								setDeclaring(target, declared);
							}

							return declared;
						};
				}
			}

			throw new NotImplementedException("Have not gotten this far yet");
		}

		private static Func<object> GetObjectFactory(Type type)
		{
			if (type.IsGenericType)
			{
				Type genericType = type.GetGenericTypeDefinition();
				if (genericType == typeof (IList<>))
				{
					Type createType = typeof (List<>).MakeGenericType(type.GetGenericArguments()[0]);

					return () => FastActivator.Create(createType);
				}
			}

			return () => FastActivator.Create(type);
		}
	}
}
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
namespace Magnum.Specs.Monads
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	using Magnum.Extensions;
	using Magnum.Reflection;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Settings_a_property_on_a_missing_member
	{
		[Test]
		public void Should_allow_property_to_already_be_set()
		{
			Expression<Func<OuterClass, object>> accessor = o => o.Inner.Value;

			Action<OuterClass, object> writer = accessor.CreateSafeSetter();

			const string expected = "Hello";

			var subject = new OuterClass();
			subject.Inner = new InnerClass {OtherValue = "Hi"};

			writer(subject, expected);

			subject.Inner.Value.ShouldEqual(expected);
			subject.Inner.OtherValue.ShouldEqual("Hi");
		}

		[Test]
		public void Should_create_a_setter_for_a_simple_value()
		{
			Expression<Func<OuterClass, object>> accessor = o => o.Value;


			Action<OuterClass, object> writer = accessor.CreateSafeSetter();


			var subject = new OuterClass();

			const string expected = "Hello";

			writer(subject, expected);

			subject.Value.ShouldEqual(expected);
		}

		[Test]
		public void Should_go_way_deep()
		{
			Expression<Func<WayOuterClass, object>> accessor = o => o.Outer.Inner.Value;

			Action<WayOuterClass, object> writer = accessor.CreateSafeSetter();

			const string expected = "Hello";

			var subject = new WayOuterClass();

			writer(subject, expected);

			subject.Outer.Inner.Value.ShouldEqual(expected);
		}

		[Test]
		public void Should_not_throw_a_null_reference_exception()
		{
			Expression<Func<OuterClass, object>> accessor = o => o.Inner.Value;

			Action<OuterClass, object> writer = accessor.CreateSafeSetter();

			const string expected = "Hello";

			var subject = new OuterClass();

			writer(subject, expected);

			subject.Inner.Value.ShouldEqual(expected);
		}

		public class InnerClass
		{
			public string Value { get; set; }
			public string OtherValue { get; set; }
		}

		public class OuterClass
		{
			public InnerClass Inner { get; set; }
			public string Value { get; set; }
		}

		public class WayOuterClass
		{
			public OuterClass Outer { get; private set; }
		}
	}

	public static class extension
	{
		public static Action<T, object> CreateSafeSetter<T>(this Expression<Func<T, object>> expression)
		{
			return new SetterMaker().CreateSetter(expression);

			//return new CreateSetterExpressionVisitor<T>().CreateSafeSetter(expression);
		}
	}


	public class SetterMaker
	{
		public Action<T, object> CreateSetter<T>(Expression<Func<T, object>> expression)
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
					return this.FastInvoke<SetterMaker, Action<T, object>>(new[] {typeof (T), me.Expression.Type},
					                                                       "CreateNullSafeSetter",
					                                                       property, me.Expression, expression.Parameters[0]);
				}
			}

			throw new NotImplementedException("Not ready yet.");
		}

		private Action<T, object> CreateNullSafeSetter<T, TDeclaring>(PropertyInfo propertyInfo,
		                                                              MemberExpression me,
		                                                              ParameterExpression parameter)
			where TDeclaring : class, new()
		{
			Func<T, TDeclaring> getDeclaring = CreateNullSafeGetter<T, TDeclaring>(me, parameter);

			Action<TDeclaring, object> setProperty = FastProperty<TDeclaring>.GetSetMethod(propertyInfo, true);

			return (o, v) =>
				{
					TDeclaring declared = getDeclaring(o);

					setProperty(declared, v);
				};
		}

		private Func<T, TDeclaring> CreateNullSafeGetter<T, TDeclaring>(MemberExpression me,
		                                                                ParameterExpression parameter)
			where TDeclaring : class, new()
		{
			if (me.Member.MemberType == MemberTypes.Property)
			{
				var property = (PropertyInfo) me.Member;

				if (me.Expression.NodeType == ExpressionType.Parameter && me.Expression == parameter)
				{
					Func<T, TDeclaring> getDeclaring = FastProperty<T, TDeclaring>.GetGetMethod(property);
					Action<T, TDeclaring> setDeclaring = FastProperty<T, TDeclaring>.GetSetMethod(property, true);

					return (o) =>
						{
							TDeclaring declared = getDeclaring(o);
							if (declared == null)
							{
								declared = new TDeclaring();
								setDeclaring(o, declared);
							}

							return declared;
						};
				}

				if (me.Expression.NodeType == ExpressionType.MemberAccess)
				{
//					return this.FastInvoke<SetterMaker, Action<T, object>>(new[] {typeof (T), me.Expression.Type},
//					                                                       "CreateNullSafeSetter",
//					                                                       property, me.Expression, expression.Parameters[0]);
				}
			}

			throw new NotImplementedException("Have not gotten this far yet");
		}
	}


	public class CreateSetterExpressionVisitor<T> :
		ExpressionVisitor
	{
		private ParameterExpression _parameter;
		private Action<T, object> _result;

		public Action<T, object> CreateSafeSetter(Expression<Func<T, object>> expression)
		{
			_parameter = expression.Parameters[0];

			return DoIt(expression);
		}

		private Action<T, object> DoIt(Expression<Func<T, object>> expression)
		{
			Visit(expression);

			if (_result == null)
				throw new InvalidOperationException("The expression was not evaluated into a proper setter");

			return _result;
		}

		protected override Expression VisitMethodCall(MethodCallExpression m)
		{
			return base.VisitMethodCall(m);
		}

		protected override Expression VisitMemberAccess(MemberExpression m)
		{
			if (m.Member.MemberType == MemberTypes.Property)
			{
				var property = (PropertyInfo) m.Member;

				if (m.Expression.NodeType == ExpressionType.Parameter)
				{
					_result = GetSetMethod(property, true);
				}
				else
				{
					this.FastInvoke(new[] {m.Expression.Type}, "VisitClassProperty", new object[] {property, m.Expression});
				}
			}

			return base.VisitMemberAccess(m);
		}

		private void VisitClassProperty<TContainer>(PropertyInfo property, Expression expression)
			where TContainer : class
		{
			Expression<Func<T, TContainer>> inputExpression = Expression.Lambda<Func<T, TContainer>>(expression, _parameter);

			Func<T, TContainer> input = inputExpression.Compile();

			Func<TContainer, object> getter = GetGetMethod<TContainer>(property);
			Action<TContainer, object> setter = GetSetMethod<TContainer>(property, true);
			Action<T, object> writer = GetSetMethod(property, true);

			Type propertyType = property.PropertyType;

			_result = (x, v) =>
				{
					TContainer p = input(x);

					object current = getter(p);
					if (current == null)
					{
						current = FastActivator.Create(propertyType);
						setter(p, current);
					}
				};
		}

		private static Action<T, object> GetSetMethod(PropertyInfo property, bool includeNonPublic)
		{
			if (!property.CanWrite)
				return (x, i) => { throw new InvalidOperationException("No setter available on " + property.Name); };

			ParameterExpression instance = Expression.Parameter(typeof (T), "instance");
			ParameterExpression value = Expression.Parameter(typeof (object), "value");
			UnaryExpression valueCast;
			if (property.PropertyType.IsValueType)
				valueCast = Expression.Convert(value, property.PropertyType);
			else
				valueCast = Expression.TypeAs(value, property.PropertyType);

			MethodCallExpression call = Expression.Call(instance, property.GetSetMethod(includeNonPublic), valueCast);

			return Expression.Lambda<Action<T, object>>(call, new[] {instance, value}).Compile();
		}

		private static Action<T1, object> GetSetMethod<T1>(PropertyInfo property, bool includeNonPublic)
		{
			if (!property.CanWrite)
				return (x, i) => { throw new InvalidOperationException("No setter available on " + property.Name); };

			ParameterExpression instance = Expression.Parameter(typeof (T1), "instance");
			ParameterExpression value = Expression.Parameter(typeof (object), "value");
			UnaryExpression valueCast;
			if (property.PropertyType.IsValueType)
				valueCast = Expression.Convert(value, property.PropertyType);
			else
				valueCast = Expression.TypeAs(value, property.PropertyType);

			MethodCallExpression call = Expression.Call(instance, property.GetSetMethod(includeNonPublic), valueCast);

			return Expression.Lambda<Action<T1, object>>(call, new[] {instance, value}).Compile();
		}

		private static Func<T, object> GetGetMethod(PropertyInfo property)
		{
			ParameterExpression instance = Expression.Parameter(typeof (T), "instance");
			MethodCallExpression call = Expression.Call(instance, property.GetGetMethod());
			UnaryExpression typeAs = Expression.TypeAs(call, typeof (object));
			return Expression.Lambda<Func<T, object>>(typeAs, instance).Compile();
		}

		private static Func<T1, object> GetGetMethod<T1>(PropertyInfo property)
		{
			ParameterExpression instance = Expression.Parameter(typeof (T1), "instance");
			MethodCallExpression call = Expression.Call(instance, property.GetGetMethod());
			UnaryExpression typeAs = Expression.TypeAs(call, typeof (object));
			return Expression.Lambda<Func<T1, object>>(typeAs, instance).Compile();
		}
	}
}
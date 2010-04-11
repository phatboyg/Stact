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
namespace Magnum.Web.Binding
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Channels;
	using Extensions;
	using InterfaceExtensions;
	using Logging;
	using Reflection;
	using TypeBinders;

	public class InstanceBinderContext :
		BinderContext
	{
		private static readonly ILogger _log = Logger.GetLogger<InstanceBinderContext>();
		private static readonly Dictionary<Type, ObjectBinder> _typeBinders;

		private readonly Stack<ModelBinderContext> _contextStack;
		private readonly Stack<ObjectPropertyBinder> _propertyStack;

		static InstanceBinderContext()
		{
			_typeBinders = new Dictionary<Type, ObjectBinder>();

			LoadBuiltInBinders();
		}

		public InstanceBinderContext(ModelBinderContext context)
		{
			_contextStack = new Stack<ModelBinderContext>();
			_contextStack.Push(context);

			_propertyStack = new Stack<ObjectPropertyBinder>();
		}

		public ModelBinderContext Context
		{
			get { return _contextStack.Count > 0 ? _contextStack.Peek() : null; }
		}

		public string PropertyKey
		{
			get
			{
				const string separator = "_";

				string value = string.Join(separator,
					_propertyStack
						.Select(x => x.Property.Name)
						.Reverse()
						.ToArray());

				return value;
			}
		}

		public object PropertyValue
		{
			get
			{
				object value = null;

				Context.GetValue(PropertyKey, x =>
					{
						value = x;
						return true;
					}, () => value = null);

				return value;
			}
		}

		public PropertyInfo Property
		{
			get { return _propertyStack.Count > 0 ? _propertyStack.Peek().Property : null; }
		}

		public object Bind(Type type)
		{
			ObjectBinder binder;
			lock (_typeBinders)
			{
				binder = _typeBinders.Retrieve(type, () => CreateBinderFor(type));
			}

			return binder.Bind(this);
		}

		public object Bind(ObjectPropertyBinder property)
		{
			_propertyStack.Push(property);

			try
			{
				return Bind(Property.PropertyType);
			}
			finally
			{
				_propertyStack.Pop();
			}
		}

		public Channel<T> GetChannel<T>()
		{
			return Context.GetChannel<T>();
		}

		private static void LoadBuiltInBinders()
		{
			Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(x => !x.IsGenericType)
				.Where(x => x.Namespace == typeof (StringBinder).Namespace)
				.Each(type =>
					{
						type.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (ObjectBinder<>))
							.Each(interfaceType =>
								{
									Type itemType = interfaceType.GetGenericArguments().First();

									_log.Debug(x => x.Write("Loading binder or {0} ({1})", itemType.Name, type.Name));

									_typeBinders.Add(itemType, FastActivator.Create(type) as ObjectBinder);
								});
					});
		}

		private static ObjectBinder CreateBinderFor(Type type)
		{
			if (type.Implements<Channel>())
			{
				Type[] genericType = type.GetGenericArguments();

				return (ObjectBinder) FastActivator.Create(typeof (ChannelBinder<>).MakeGenericType(genericType));
			}

			if (type.IsEnum)
			{
				return (ObjectBinder) FastActivator.Create(typeof (EnumBinder<>).MakeGenericType(type));
			}

			if (typeof (IEnumerable).IsAssignableFrom(type) && type != typeof (string))
			{
				if (type.IsArray)
				{
					return (ObjectBinder) FastActivator.Create(typeof (ArrayBinder<>).MakeGenericType(type.GetElementType()));
				}
				if (type.IsGenericType)
				{
					Type genericTypeDefinition = type.GetGenericTypeDefinition();
					Type[] arguments = type.GetGenericArguments();
					if (genericTypeDefinition == typeof (IList<>) || genericTypeDefinition == typeof (List<>))
					{
						return (ObjectBinder) FastActivator.Create(typeof (ListBinder<>).MakeGenericType(arguments));
					}

//					if (genericTypeDefinition == typeof (IDictionary<,>) || genericTypeDefinition == typeof (Dictionary<,>))
//					{
//						return (ObjectBinder) FastActivator.Create(typeof (DictionaryBinder<,>).MakeGenericType(arguments));
//					}
				}

				throw new NotSupportedException("Unsupported enumeration type: " + type.FullName);
			}

			Type binderType;
			if (type.IsInterface)
			{
				Type proxyType = InterfaceImplementationBuilder.GetProxyFor(type);
				binderType = typeof (FastObjectBinder<>).MakeGenericType(proxyType);
			}
			else
			{
				binderType = typeof (FastObjectBinder<>).MakeGenericType(type);
			}

			return (ObjectBinder) FastActivator.Create(binderType);
		}
	}
}
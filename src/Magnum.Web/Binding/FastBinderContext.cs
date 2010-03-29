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
namespace MassTransit.Serialization.Custom
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Xml;
	using Magnum;
	using Magnum.CollectionExtensions;
	using Magnum.Logging;
	using Magnum.Reflection;
	using Magnum.Web.Binding;
	using Magnum.Web.Binding.TypeBinders;

	public class FastBinderContext :
		BinderContext

	{
		private readonly ModelBinderContext _context;
		private static readonly Dictionary<Type, ObjectBinder> _binders;
		private static readonly ILogger _log = Logger.GetLogger<FastBinderContext>();
		private readonly XmlReader _reader;

		static FastBinderContext()
		{
			_binders = new Dictionary<Type, ObjectBinder>();

			LoadBuiltInBinders();
		}

		public object PropertyValue { get; protected set; }

		private readonly Stack<ObjectPropertyBinder> _propertyStack;


		public PropertyInfo Property
		{
			get { return _propertyStack.Peek().Property; }
		}

	
		public FastBinderContext(ModelBinderContext context)
		{
			_context = context;
		}

		public object Bind(Type type)
		{
			ObjectBinder binder;
			lock (_binders)
			{
				binder = _binders.Retrieve(type, () => CreateBinderFor(type));
			}

			return binder.Bind(this);
		}

		public object Bind(PropertyInfo property)
		{
			object value = _context.Values.GetValue(property.Name);

			PropertyValue = value;

			return Bind(property.PropertyType);
		}

		public void Bind(ObjectPropertyBinder property)
		{
		}

		public string ReadElementAsString()
		{
			return PropertyValue.ToString();
		}

		private static void LoadBuiltInBinders()
		{
			Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(x => !x.IsGenericType)
				.Where(x => x.Namespace == typeof (StringBinder).Namespace)
				.Each(type =>
					{
						type.GetInterfaces()
							.Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (ObjectBinder<>))
							.Each(interfaceType =>
								{
									Type itemType = interfaceType.GetGenericArguments().First();

									_log.Debug(x => x.Write("Loading binder or {0} ({1})", itemType.Name, type.Name));

									_binders.Add(itemType, FastActivator.Create(type) as ObjectBinder);
								});
					});
		}

		private static ObjectBinder CreateBinderFor(Type type)
		{
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
// Copyright 2010 Chris Patterson
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
namespace Stact.Specs.Headers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Magnum.Extensions;
	using Magnum.Reflection;
	using Magnum.TestFramework;
	using MessageHeaders;
	using NUnit.Framework;


	public static class DynamicMessageInitializer
	{
		[ThreadStatic]
		static MatchHeader _matchHeader;

		[ThreadStatic]
		static IDictionary<Type, MessageInitializerFactory> _initializers;

		public static void Initialize<TInput>(Action<MessageInitializer<TInput>> initializer, Action<TInput> callback)
		{
			var factory = IsInCache(typeof(TInput), x => new DynamicMessageInitializerFactory<TInput>());

			factory.Initialize(initializer, callback);
		}

		static MessageInitializerFactory IsInCache(Type type, Func<Type, MessageInitializerFactory> provider)
		{
			if (_initializers == null)
				_initializers = new Dictionary<Type, MessageInitializerFactory>();

			MessageInitializerFactory factory;
			if (_initializers.TryGetValue(type, out factory))
				return factory;

			factory = provider(type);

			_initializers[type] = factory;

			return factory;
		}

		static void Match<TInput>(TInput message, MatchHeaderCallback callback)
		{
			if (_matchHeader == null)
				_matchHeader = new MatchHeaderImpl();

			_matchHeader.Match(message, callback);
		}
	
	}

	public interface MessageInitializerFactory
	{
		void Initialize<TInput>(Action<MessageInitializer<TInput>> initializer, Action<TInput> callback);
	}

	public class DynamicMessageInitializerFactory<T> :
		MessageInitializerFactory
	{
		IDictionary<string, FastProperty<T>> _properties;

		public DynamicMessageInitializerFactory()
		{
			_properties = GetProperties();

		}

		private static IDictionary<string, FastProperty<T>> GetProperties()
		{
			return typeof(T).GetAllProperties()
				.Where(x => x.GetGetMethod() != null)
				.Where(x => x.GetSetMethod(true) != null || typeof(T).IsInterface)
				.Select(x => new { x.Name, fp = new FastProperty<T>(x)})
				.ToDictionary(x => x.Name, x => x.fp);
		}

		public void Initialize<TInput>(Action<MessageInitializer<TInput>> initializer, Action<TInput> callback)
		{
			var initialize = new DynamicMessageInitializer<T>(_properties);

			initializer((MessageInitializer<TInput>)initialize);

			callback((TInput)initialize.GetMessage());
		}
	}

	public class DynamicMessageInitializer<T> :
 		MessageInitializer<T>
	{
		readonly IDictionary<string, FastProperty<T>> _properties;
		T _message;

		public DynamicMessageInitializer(IDictionary<string, FastProperty<T>> properties)
		{
			_properties = properties;
			if (!typeof(T).IsInterface)
				throw new ArgumentException("Default Implementations can only be created for interfaces");

			Type requestImplType = InterfaceImplementationBuilder.GetProxyFor(typeof(T));

			_message = (T)FastActivator.Create(requestImplType);
		}

		public void Set<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
		{
			var fastProperty = _properties[property.MemberName()];

			fastProperty.Set(_message, value);
		}


		public object GetMessage()
		{
			return _message;
		}
	}


	public interface MessageInitializer
	{
		
	}

	public interface MessageInitializer<T> :
		MessageInitializer
	{
		void Set<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);
	}

	public static class bueriuower
	{
		public static void Send<T>(this UntypedChannel channel, Action<MessageInitializer<T>> initializer)
		{
			DynamicMessageInitializer.Initialize(initializer, channel.Send);

		}
	}

	[Scenario]
	public class Using_a_message_proxy
	{
		[Then, Explicit]
		public void Should_properly_setup_the_message_body()
		{
			ChannelAdapter channel = new ChannelAdapter();


			channel.Send<Message<MyBody>>(msg =>
				{
				});

		}

		interface MyBody
		{
			string Name { get; }
			int Id { get; }
		}
	}
}
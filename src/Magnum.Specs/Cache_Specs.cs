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
namespace Magnum.Common.Specs
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;
	using Machine.Specifications;
	using Magnum.Specs;

    [Concern("Caching")]
	public class when_a_cache_is_defined
	{
		private static UserCache _userCache;

		private Establish context = () =>
			{
				User one = new User {Id = 27, Name = "Chris"};

				_userCache = new UserCache {one};
			};

		private Because of = () => {};

		private It the_items_in_the_cache_should_exist = () => {SpecExtensions.ShouldBeTrue(_userCache.Exists(27));};
	}

	internal class UserCache : CacheBase<User>
	{
		public UserCache()
		{
			Id(u => u.Id);
		}
	}

	public class CacheBase<T> : IEnumerable<T>
	{
		private readonly List<ICacheIndex<T>> _indices = new List<ICacheIndex<T>>();
		private readonly HashSet<T> _items = new HashSet<T>();

		public IEnumerator<T> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		protected void Id<V>(Expression<Func<T, V>> expression)
		{
			MemberExpression memberExpression = GetMemberExpression(expression);

			Type t = typeof (CacheIndex<,>).MakeGenericType(typeof (T), ((PropertyInfo) memberExpression.Member).PropertyType);

			ICacheIndex<T> index = (ICacheIndex<T>) Activator.CreateInstance(t, expression);

			_indices.Add(index);
		}

		private static MemberExpression GetMemberExpression<TX, VX>(Expression<Func<TX, VX>> expression)
		{
			MemberExpression memberExpression = null;
			if (expression.Body.NodeType == ExpressionType.Convert)
			{
				var body = (UnaryExpression) expression.Body;
				memberExpression = body.Operand as MemberExpression;
			}
			else if (expression.Body.NodeType == ExpressionType.MemberAccess)
			{
				memberExpression = expression.Body as MemberExpression;
			}

			if (memberExpression == null)
				throw new ArgumentException("Not a member access", "member");

			return memberExpression;
		}

		public void Add(T item)
		{
			_items.Add(item);
			foreach (ICacheIndex<T> index in _indices)
			{
				index.Add(item);
			}
		}

		public bool Exists<V>(V key)
		{
			return _indices[0].Contains(key);
		}

	}

	internal class CacheIndex<T, V> : ICacheIndex<T>
	{
		private readonly Func<T, V> _accessor;
		private readonly Dictionary<V, T> _index = new Dictionary<V, T>();

		public CacheIndex(Expression<Func<T, V>> accessor)
		{
			_accessor = accessor.Compile();
		}

		public void Add(T item)
		{
			_index.Add(_accessor(item), item);
		}

		public T this[object key]
		{
			get
			{
				if (key.GetType() != typeof(V))
					throw new ArgumentException("Unknown argument type specified");

				V vKey = (V)key;

				if (_index.ContainsKey(vKey) == false)
					return default(T);

				return _index[vKey];
			}
		}

		public bool Contains(object key)
		{
			if (key.GetType() != typeof(V))
				throw new ArgumentException("Unknown argument type specified");

			V vKey = (V)key;

			return _index.ContainsKey(vKey);
		}
	}

	internal interface ICacheIndex<T>
	{
		T this[object key] { get; }
		void Add(T item);

		bool Contains(object key);
	}

	internal class User
	{
		public int Id { get; set; }

		public string Name { get; set; }
	}
}
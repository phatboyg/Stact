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
namespace Magnum.Common
{
	using System;
	using System.Collections.Generic;

	public class Mapper<TSource, TTarget>
		where TTarget : new()
	{
		private readonly List<IMapAction<TSource, TTarget>> _maps = new List<IMapAction<TSource, TTarget>>();

		public MapAction<TProperty> From<TProperty>(Func<TSource, TProperty> property)
		{
			return new MapAction<TProperty>(this, property);
		}

		private void Add(IMapAction<TSource, TTarget> mapAction)
		{
			_maps.Add(mapAction);
		}

		public TTarget Transform(TSource source)
		{
			return Transform(source, new TTarget());
		}

		public TTarget Transform(TSource source, TTarget target)
		{
			foreach (IMapAction<TSource, TTarget> mapAction in _maps)
			{
				mapAction.Map(source, target);
			}

			return target;
		}

		public interface IMapAction<TS, TT>
		{
			void Map(TS source, TT target);
		}

		public class MapAction<TProperty> :
			IMapAction<TSource, TTarget>
		{
			private readonly Mapper<TSource, TTarget> _mapper;
			private readonly Func<TSource, TProperty> _property;
			private Action<TTarget, TProperty> _action;

			internal MapAction(Mapper<TSource, TTarget> mapper, Func<TSource, TProperty> property)
			{
				_mapper = mapper;
				_property = property;
			}

			public void Map(TSource source, TTarget target)
			{
				_action(target, _property(source));
			}

			public MapAction<TProperty> To(Action<TTarget, TProperty> action)
			{
				_action = action;

				_mapper.Add(this);
				return this;
			}
		}
	}
}
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
namespace Stact.Configuration.Internal
{
	using System.Linq.Expressions;
	using System.Reflection;
	using Builders;


	public class PropertyChannelConfiguratorImpl<T, TChannel> :
		PropertyChannelConfigurator<T>
		where T : class
	{
		ChannelAccessor<T, TChannel> _accessor;

		public PropertyChannelConfiguratorImpl(PropertyInfo property)
		{
			_accessor = CreateAccessor(property);
		}

		public void Configure(ConnectionBuilder builder, T instance)
		{
			Channel<TChannel> channel = _accessor(instance);
			if (channel != null)
				builder.AddChannel(channel);
		}

		static ChannelAccessor<T, TChannel> CreateAccessor(PropertyInfo property)
		{
			ParameterExpression target = Expression.Parameter(typeof(T), "x");
			MethodCallExpression getter = Expression.Call(target, property.GetGetMethod(true));

			return Expression.Lambda<ChannelAccessor<T, TChannel>>(getter, new[] {target})
				.Compile();
		}
	}
}
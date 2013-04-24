// Copyright 2010-2013 Chris Patterson
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
namespace Stact.Internal.TypeConverters
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Internals.Extensions;
    using MessageHeaders;


    public class MessageUpConverterFactory<T> :
        HeaderTypeConverterFactory<T>
    {
        readonly Func<object, T> _converter;
        readonly Type _messageType;
        readonly bool _supported;

        public MessageUpConverterFactory()
        {
            _supported = typeof(T).IsGenericType && typeof(Message<>).Equals(typeof(T).GetGenericTypeDefinition());
            if (!_supported)
                return;

            _messageType = typeof(T).GetClosingArguments(typeof(Message<>)).Single();

            _converter = GenerateConverterMethod(_messageType);
        }

        public bool CanConvert<TInput>(TInput input, out Func<object, T> converter)
        {
            converter = null;

            if (!_supported)
                return false;

            if (!_messageType.IsAssignableFrom(typeof(TInput)))
                return false;

            converter = _converter;
            return true;
        }

        static Func<object, T> GenerateConverterMethod(Type messageType)
        {
            ParameterExpression value = Expression.Parameter(typeof(object), "value");

            UnaryExpression castValue;
            if (typeof(T).IsValueType)
                castValue = Expression.Convert(value, messageType);
            else
                castValue = Expression.TypeAs(value, messageType);

            Type messageImplType = typeof(MessageContext<>).MakeGenericType(messageType);

            ConstructorInfo constructorInfo = messageImplType.GetConstructor(new[] {messageType});

            NewExpression constructor = Expression.New(constructorInfo, castValue);

            Expression<Func<object, T>> expression = Expression.Lambda<Func<object, T>>(constructor, value);

            return expression.Compile();
        }
    }
}
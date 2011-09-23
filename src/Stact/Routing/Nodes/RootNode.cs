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
namespace Stact.Routing.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.Caching;
    using Magnum.Extensions;


    public class RootNode :
        Activation
    {
        static readonly Cache<Type, AlphaNodeInitializer> _initializers =
            new GenericTypeCache<AlphaNodeInitializer>(typeof(AlphaNodeInitializerImpl<>));

        readonly Cache<Type, Activation> _types;

        public RootNode()
        {
            _types = new GenericTypeCache<Activation>(typeof(AlphaNode<>));
        }

        public IEnumerable<Activation> Activations
        {
            get { return _types; }
        }

        public void Activate<T>(RoutingContext<T> context)
        {
            _types.Get(typeof(T), CreateMissingAlphaNode<T>).Activate(context);
        }

        Activation CreateMissingAlphaNode<T>(Type type)
        {
            var alphaNode = new AlphaNode<T>();

            foreach (Type nestedType in GetNestedMessageTypes(typeof(T)))
                _initializers[nestedType].AddActivation(this, alphaNode);

            return alphaNode;
        }

        public AlphaNode<T> GetAlphaNode<T>()
        {
            var value = _types.Get(typeof(T), CreateMissingAlphaNode<T>) as AlphaNode<T>;
            if (value != null)
                return value;

            throw new InvalidOperationException(
                "The activation for {0} is not an Alpha node".FormatWith(typeof(T).ToShortTypeName()));
        }

        public static IEnumerable<Type> GetNestedMessageTypes(Type type)
        {
            IEnumerable<Type> excludedInterfaces = Enumerable.Empty<Type>();
            Type baseType = type.BaseType;
            if ((baseType != null) && IsAllowedMessageType(baseType))
            {
                yield return baseType;

                excludedInterfaces = baseType.GetInterfaces();
            }

            IEnumerable<Type> interfaces = type
                .GetInterfaces()
                .Except(excludedInterfaces)
                .Where(IsAllowedMessageType);

            foreach (Type interfaceType in interfaces)
                yield return interfaceType;
        }

        public static bool IsAllowedMessageType(Type type)
        {
            if (type.Namespace == null)
                return false;

            if (type == typeof(Message)
                || type == typeof(MessageHeader)
                || type == typeof(RequestHeader)
                || type == typeof(ResponseHeader))
                return false;

            if (type.Assembly == typeof(object).Assembly)
                return false;

            if (type.Namespace == "System")
                return false;

            if (type.Namespace.StartsWith("System."))
                return false;

            return true;
        }
    }
}
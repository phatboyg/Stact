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
namespace Stact.Actors.Behaviors
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Internals.Extensions;


    public class MessageOnlyMethodApplicator<TState, TBehavior, TMessage> :
        ActorBehaviorApplicator<TState, TBehavior>
        where TBehavior : Behavior<TState>
    {
        readonly Action<TBehavior, Message<TMessage>> _consumer;

        public MessageOnlyMethodApplicator(MethodInfo method)
        {
            _consumer = GenerateConsumer(method);
        }

        public void Apply(BehaviorContext<TState, TBehavior> context)
        {
            context.Receive<TMessage>(message => _consumer(context.Behavior, message));
        }

        static Action<TBehavior, Message<TMessage>> GenerateConsumer(MethodInfo method)
        {
            ParameterExpression behavior = Expression.Parameter(typeof(TBehavior), "behavior");
            ParameterExpression message = Expression.Parameter(typeof(Message<TMessage>), "message");

            Type messageParameterType = method.GetParameters()[0].ParameterType;

            var args = new[] {behavior, message};

            if (messageParameterType == typeof(Message<TMessage>))
            {
                MethodCallExpression call = Expression.Call(behavior, method, message);

                return Expression.Lambda<Action<TBehavior, Message<TMessage>>>(call, args).Compile();
            }

            if (messageParameterType == typeof(TMessage))
            {
                const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

                PropertyInfo bodyProperty = typeof(Message<TMessage>).GetProperty("Body", bindingFlags);
                MethodCallExpression body = Expression.Call(message, bodyProperty.GetGetMethod(true));

                MethodCallExpression call = Expression.Call(behavior, method, body);

                return Expression.Lambda<Action<TBehavior, Message<TMessage>>>(call, args).Compile();
            }

            throw new ArgumentException("The argument for the method is not assignable from the message type: " +
                                        typeof(TMessage).GetTypeName());
        }
    }
}
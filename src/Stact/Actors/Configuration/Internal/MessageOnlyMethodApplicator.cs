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
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Behaviors;


    public class MessageOnlyMethodApplicator<TState, TBehavior, TMessage> :
        ActorBehaviorApplicator<TState, TBehavior>
        where TBehavior : Behavior<TState>
    {
        Action<TBehavior, Message<TMessage>> _consumer;

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
            MethodCallExpression call = Expression.Call(behavior, method, message);

            return Expression.Lambda<Action<TBehavior, Message<TMessage>>>(call, new[] { behavior, message }).Compile();
        }
    }
}
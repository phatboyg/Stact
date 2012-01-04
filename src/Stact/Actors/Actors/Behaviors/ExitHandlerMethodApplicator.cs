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


    public class ExitHandlerMethodApplicator<TState, TBehavior> :
        ActorBehaviorApplicator<TState, TBehavior>
        where TBehavior : Behavior<TState>
    {
        Action<TBehavior, Message<Exit>, NextExitHandler> _handler;

        public ExitHandlerMethodApplicator(MethodInfo method)
        {
            _handler = GenerateHandler(method);
        }

        public void Apply(BehaviorContext<TState, TBehavior> context)
        {
            context.SetExitHandler((message, next) => _handler(context.Behavior, message, next));
        }

        static Action<TBehavior, Message<Exit>, NextExitHandler> GenerateHandler(MethodInfo method)
        {
            ParameterExpression behavior = Expression.Parameter(typeof(TBehavior), "behavior");
            ParameterExpression exception = Expression.Parameter(typeof(Message<Exit>), "message");
            ParameterExpression next = Expression.Parameter(typeof(NextExitHandler), "next");
            MethodCallExpression call = Expression.Call(behavior, method, exception, next);

            var parameters = new[] {behavior, exception, next};
            return Expression.Lambda<Action<TBehavior, Message<Exit>, NextExitHandler>>(call, parameters)
                .Compile();
        }
    }
}
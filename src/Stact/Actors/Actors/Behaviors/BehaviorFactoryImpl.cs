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
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Internals.Extensions;


    public class BehaviorFactoryImpl<TState, TBehavior> :
        BehaviorFactory<TState, TBehavior>
        where TBehavior : class, Behavior<TState>
    {
        readonly Func<Actor<TState>, TBehavior> _factory;

        public BehaviorFactoryImpl()
        {
            _factory = GenerateBehaviorFactory();
        }

        public TResult CreateBehavior<TResult>(Actor<TState> state, Func<TBehavior, TResult> callback)
        {
            TBehavior behavior = _factory(state);
            if (behavior == null)
                throw new StactException("The behavior factory returned null: " + typeof(TBehavior).GetTypeName());

            return callback(behavior);
        }

        Func<Actor<TState>, TBehavior> GenerateBehaviorFactory()
        {
            Debug.WriteLine("Locating constructor for {0}", (object)typeof(TBehavior).GetTypeName());

            ConstructorInfo constructorInfo = typeof(TBehavior)
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetParameters().Count() == 1)
                .Where(x => x.GetParameters().Single().ParameterType == typeof(Actor<TState>))
                .SingleOrDefault();
            if (constructorInfo == null)
                throw new StactException("No valid constructor was found for the behavior: " + typeof(TBehavior).Name);

            ParameterExpression actor = Expression.Parameter(typeof(Actor<TState>), "actor");
            NewExpression construct = Expression.New(constructorInfo, actor);
            Expression<Func<Actor<TState>, TBehavior>> lambda =
                Expression.Lambda<Func<Actor<TState>, TBehavior>>(construct, actor);

            return lambda.Compile();
        }
    }
}
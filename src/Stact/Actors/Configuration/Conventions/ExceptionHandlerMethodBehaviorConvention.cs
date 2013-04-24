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
namespace Stact.Configuration.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Actors.Behaviors;
    using Internals.Extensions;


    public class ExceptionHandlerMethodBehaviorConvention :
        BehaviorConvention
    {
        public IEnumerable<ActorBehaviorApplicator<TState, TBehavior>> GetApplicators<TState, TBehavior>()
            where TBehavior : Behavior<TState>
        {
            return typeof(TBehavior)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetParameters().Count() == 2)
                .Where(x => x.GetParameters()[0].ParameterType == typeof(Exception))
                .Where(x => x.GetParameters()[1].ParameterType == typeof(NextExceptionHandler))
                .Select(CreateMethodConvention<TState, TBehavior>);
        }

        static ActorBehaviorApplicator<TState, TBehavior> CreateMethodConvention<TState, TBehavior>(MethodInfo method)
            where TBehavior : Behavior<TState>
        {
            Debug.WriteLine("Creating applicator for {0}, exception handler: {1}({2},{3})",
                            typeof(TBehavior).GetTypeName(),
                            method.Name, typeof(Exception).GetTypeName(),
                            typeof(NextExceptionHandler).GetTypeName());

            var genericTypes = new[] {typeof(TState), typeof(TBehavior)};

            var args = new object[] {method};

            return (ActorBehaviorApplicator<TState, TBehavior>)
                   Activator.CreateInstance(typeof(ExceptionHandlerMethodApplicator<,>).MakeGenericType(genericTypes), args);
        }
    }
}
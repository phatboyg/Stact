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
namespace Stact
{
    using System;


    public static class ActorExceptionHandlerExtensions
    {
        public static void SendExceptionsAsFaults<TState>(this Actor<TState> actor)
        {
            actor.SetExceptionHandler((exception, next) => ExceptionHandler(actor, exception));
        }

        static Message<Fault> ExceptionHandler<TState>(Actor<TState> actor, Exception exception)
        {
            return actor.Self.Send<Fault>(new FaultImpl(exception), actor.Self);
        }


        class FaultImpl :
            Fault
        {
            readonly Exception _exception;

            public FaultImpl(Exception exception)
            {
                _exception = exception;
            }

            public string Message
            {
                get { return _exception.Message; }
            }

            public string StackTrace
            {
                get { return _exception.StackTrace; }
            }
        }
    }
}
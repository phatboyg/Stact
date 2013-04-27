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
    using System.Threading;


    /// <summary>
    ///   A synchronous fiber will execute an action immediately on the calling thread
    ///   without any protection from an exception
    /// </summary>
    public class SynchronousFiber :
        Fiber,
        ExecutionContext
    {
        static readonly CancellationToken _none = CancellationToken.None;

        bool _stopping;

        public CancellationToken CancellationToken
        {
            get { return _none; }
        }

        public void Add(Execution execution)
        {
            if (_stopping)
                return;

            if (_stopping)
                return;

            execution.Execute(this).Wait();
        }

        public void Kill()
        {
        }

        public bool Stop(TimeSpan timeout)
        {
            _stopping = true;

            return true;
        }
    }
}
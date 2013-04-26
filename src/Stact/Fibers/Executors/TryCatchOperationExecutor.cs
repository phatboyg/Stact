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
namespace Stact.Executors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class TryCatchOperationExecutor :
        OperationExecutor
    {
        readonly Action<Exception> _callback;
        bool _stopping;

        public TryCatchOperationExecutor()
        {
            _callback = x => { };
        }

        public TryCatchOperationExecutor(Action<Exception> callback)
        {
            _callback = callback;
        }


        public void Execute(Executor operation)
        {
            try
            {
                if (_stopping)
                    return;

                operation.Execute().Wait();
            }
            catch (Exception ex)
            {
                _callback(ex);
            }
        }

        public void Execute(IList<Executor> operations, Action<IEnumerable<Executor>> remaining)
        {
            int index = 0;
            try
            {
                for (index = 0; index < operations.Count; index++)
                {
                    if (_stopping)
                        break;

                    operations[index].Execute().Wait();
                }
            }
            catch (AggregateException ex)
            {
                remaining(operations.Skip(index + 1));

                _callback(ex.InnerException);
            }
        }

        public void Stop()
        {
            _stopping = true;
        }
    }
}
// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class MessageDescriptor<T>
    {
        private SortedList<int, Expression<Func<T, object>>> _lambdas = new SortedList<int, Expression<Func<T, object>>>();

        public void Serialize(CodedOutputStream outputStream, T message)
        {
            foreach (var pair in _lambdas)
            {
                var exp = pair.Value;
            }
        }

        public void AddLambda(int tag, Expression<Func<T, object>> lambda)
        {
            //ordered list? on serialization
            //this would build up a keyed list of lambdas?
            _lambdas.Add(tag, lambda);
        }
    }
}
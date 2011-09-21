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
namespace Stact.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.Caching;


    public class Agenda
    {
        Cache<int, IList<Action>> _operations;

        public Agenda()
        {
            _operations = CreateCache();
        }

        public void Add(int priority, Action operation)
        {
            _operations[priority].Add(operation);
        }

        public void Run()
        {
            if (_operations.Count == 0)
                return;

            Cache<int, IList<Action>> operations = _operations;
            _operations = CreateCache();

            foreach (Action operation in operations.GetAllKeys().OrderByDescending(x => x).SelectMany(key => operations[key]))
            {
                operation();
            }
        }

        static DictionaryCache<int, IList<Action>> CreateCache()
        {
            return new DictionaryCache<int, IList<Action>>(_ => new List<Action>());
        }
    }
}
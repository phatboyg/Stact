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


    public class Agenda
    {
        IList<KeyValuePair<int, Action>> _operations;

        bool _stopped;

        public Agenda()
        {
            _operations = new List<KeyValuePair<int, Action>>(2);
        }

        public void Add(int priority, Action operation)
        {
            if (_stopped)
                return;

            var item = new KeyValuePair<int, Action>(priority, operation);

            var count = _operations.Count;
            for (int i = 0; i < count; i++)
            {
                if (priority > _operations[i].Key)
                {
                    _operations.Insert(i, item);
                    return;
                }
            }

            _operations.Add(item);
        }

        public void Run()
        {
            if (_stopped)
                return;

            var count = _operations.Count;
            if (count == 0)
                return;

            IList<KeyValuePair<int, Action>> operations = _operations;
            _operations = new List<KeyValuePair<int, Action>>(2);

            for (int i = 0; i < count; i++)
            {
                if (_stopped)
                    break;

                operations[i].Value();
            }
        }

        public void Stop()
        {
            _stopped = true;
        }
    }
}
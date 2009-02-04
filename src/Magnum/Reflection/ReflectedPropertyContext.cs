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
namespace Magnum.Common.Reflection
{
    using System.Reflection;

    public class ReflectedPropertyContext<T> :
        IReflectedObjectContext
    {
        private readonly PropertyInfo _info;
        private readonly T _instance;

        public ReflectedPropertyContext(PropertyInfo info, T instance)
        {
            _info = info;
            _instance = instance;
        }

        public string Name
        {
            get { return _info.Name; }
        }

        public object Value
        {
            get { return ReflectionCache<T>.Get(Name, _instance); }
        }
    }
}
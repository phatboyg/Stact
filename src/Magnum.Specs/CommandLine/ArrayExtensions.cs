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
namespace Magnum.Specs.CommandLine
{
    using System;

    public static class ArrayExtensions
    {
        public static T[] Tail<T>(this T[] items) where T : class
        {
            int itemCount = items.Length == 0 ? 0 : items.Length - 1;
            T[] output = new T[itemCount];
            if(itemCount > 0)
                Array.Copy(items, 1, output, 0, itemCount);

            return output;
        }
        public static T Head<T>(this T[] items) where T : class
        {
            if (items.Length > 0)
                return items[0];

            return null;
        }
    }
}
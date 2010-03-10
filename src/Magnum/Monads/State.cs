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
namespace Magnum.Monads
{
    using System;
    using Collections;

    public delegate Tuple<STATE,CONTENT> S<STATE,CONTENT>(STATE state);

    
    //state -> state, content pair
    public static class States
    {
        //this guy captures the value of something and is able to apply state to it?
        public static S<STATE,CONTENT> ToStateMonad<STATE,CONTENT>(CONTENT content)
        {
            return s => new Tuple<STATE, CONTENT>(s,content);
        }

        public static S<STATE,CONTENTB> ToStateMonad<STATE,CONTENTA,CONTENTB>(CONTENTA a, Func<CONTENTA,CONTENTB> convert)
        {
            return s => new Tuple<STATE, CONTENTB>(s, convert(a));
        }
    }
}
// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Stact.ForNHibernate.Data.Primitives
{
    using System.Diagnostics;
    using Extensions;

    //struct primitive?
    [DebuggerDisplay("PRIM: {_value}")]
    public class Primitive<VALUE> :
        ValueObject<VALUE> where VALUE : struct
    {
        VALUE _value;

        public Primitive(VALUE value)
        {
            _value = value;
        }


        VALUE ValueObject<VALUE>.GetValue()
        {
            return _value;
        }

        public bool Equals(Primitive<VALUE> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._value.Equals(_value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Primitive<VALUE>)) return false;
            return Equals((Primitive<VALUE>) obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return "{0}='{1}'".FormatWith(this.GetType().Name, _value);
        }
    }

    public interface ValueObject<VALUE>
    {
        VALUE GetValue();
    }
}
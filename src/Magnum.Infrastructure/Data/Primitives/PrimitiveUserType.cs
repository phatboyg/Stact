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
namespace Magnum.Infrastructure.Data.Primitives
{
    using System;
    using System.Data;
    using NHibernate;
    using NHibernate.SqlTypes;
    using NHibernate.Type;
    using NHibernate.UserTypes;
    using Reflection;

    /// <summary>
	/// An NHibernate user type for storing a primitives
	/// </summary>
	[Serializable]
	public class PrimitiveUserType<PRIMITIVE, VALUE> :
		IUserType where PRIMITIVE : Primitive<VALUE> where VALUE : struct
	{
        NullableType _type;

        public PrimitiveUserType()
        {
            _type = GuessType(typeof (VALUE));
        }

        bool IUserType.Equals(object x, object y)
		{
			if (x != null)
				return x.Equals(y);

			if (y != null)
				return y.Equals(x);

			return true;
		}

		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			var value = (VALUE) _type.NullSafeGet(rs, names);

		    var primitive = FastActivator<PRIMITIVE>.Create(value);

			return primitive;
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			if (value == null)
			{
				_type.NullSafeSet(cmd, null, index);
				return;
			}

		    var primitive = (ValueObject<VALUE>) value;

		    value = primitive.GetValue();

			_type.NullSafeSet(cmd, value, index);
		}

		public object DeepCopy(object value)
		{
		    var instance = value as ValueObject<VALUE>;
            if (instance == null) return null;

			return FastActivator<PRIMITIVE>.Create(instance.GetValue());
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public object Assemble(object cached, object owner)
		{
			return cached;
		}

		public object Disassemble(object value)
		{
			return value;
		}

		public SqlType[] SqlTypes
		{
			get { return new[] {_type.SqlType}; }
		}

		public Type ReturnedType
		{
			get { return typeof (PRIMITIVE); }
		}

		public bool IsMutable
		{
			get { return false; }
		}

        private NullableType GuessType(Type t)
        {
            if (typeof(decimal).Equals(t)) return NHibernateUtil.Decimal;
            if (typeof(int).Equals(t)) return NHibernateUtil.Int32;
            if (typeof(DateTime).Equals(t)) return NHibernateUtil.DateTime;



            return NHibernateUtil.String;
        }
	}
}
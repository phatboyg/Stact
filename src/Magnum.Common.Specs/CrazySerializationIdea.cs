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
namespace Magnum.Common.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Serialization;
    using NUnit.Framework;

    [TestFixture]
    public class CrazySerializationIdea
    {
        [Test]
        public void FIRST_TEST_NAME()
        {
            CrazyMesssageMap map = new CrazyMesssageMap();

            map.Validate();
        }
    }

    internal class CrazyMessage
    {
        public Guid CorrelationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int? Age { get; set; }
        public long? Height { get; set; }
        public decimal? Amount { get; set; }

        public DateTime? Birthdate { get; set; }
    }

    internal class CrazyMesssageMap :
        MessageMap<CrazyMessage>
    {
        public CrazyMesssageMap()
        {
            Field(x => x.CorrelationId).Required();
            Field(x => x.Name).Required();
            Field(x => x.Address);
            Field(x => x.Age);
            Field(x => x.Height);
            Field(x => x.Amount);

            // new things are added with version 2

            Field(x => x.Birthdate, 2);
        }
    }

    internal class MessageMap<T>
    {
        private List<FieldMap<T>> _fields = new List<FieldMap<T>>();

        protected FieldMap<T> Field<TField>(Expression<Func<T, TField>> expression, int version)
        {
            MemberExpression memberExpression = GetMemberExpression(expression);

            FieldMap<T> fieldMap = new FieldMap<T>(memberExpression, version);

            _fields.Add(fieldMap);

            return fieldMap;
        }

        /// <summary>
        /// Maps a field as part of the message body and defaults to version 1
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected FieldMap<T> Field<TField>(Expression<Func<T, TField>> expression)
        {
            return Field(expression, 1);
        }

        private static MemberExpression GetMemberExpression<TX, VX>(Expression<Func<TX, VX>> expression)
        {
            MemberExpression memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression) expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (memberExpression == null)
                throw new ArgumentException("Not a member access", "member");

            return memberExpression;
        }

        public void Validate()
        {
            foreach (FieldMap<T> field in _fields)
            {
                field.Validate();
            }
        }
    }

    internal class FieldMap<T>
    {
        private readonly MemberExpression _expression;
        private bool _required;

        public FieldMap(MemberExpression expression, int version)
        {
            _expression = expression;
        }

        public FieldMap<T> Required()
        {
            _required = true;
            return this;
        }

        public void Validate()
        {
            Type propertyType = ((PropertyInfo) _expression.Member).PropertyType;

            if (propertyType.IsValueType)
            {
                if (_required == false)
                {
                    if (!propertyType.IsGenericType || propertyType.GetGenericTypeDefinition() != typeof (Nullable<>))
                    {
                        throw new FieldValidationException(propertyType, "Value types must be nullable if not required: " + _expression.Member.Name);
                    }
                }
            }
        }
    }

    internal class FieldValidationException : Exception
    {
        private readonly Type _type;

        public FieldValidationException(Type type, string message)
            : base(message)
        {
            _type = type;
        }

        public FieldValidationException()
        {
        }

        public FieldValidationException(string message)
            : base(message)
        {
        }

        public FieldValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected FieldValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public Type Type
        {
            get { return _type; }
        }
    }
}
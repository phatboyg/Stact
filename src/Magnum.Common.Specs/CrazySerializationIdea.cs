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
    using NUnit.Framework;

    [TestFixture]
    public class CrazySerializationIdea
    {
        [Test]
        public void FIRST_TEST_NAME()
        {
            CrazyMesssageMap map = new CrazyMesssageMap();
        }
    }

    internal class CrazyMessage
    {
        public Guid CorrelationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public long Height { get; set; }
        public decimal Amount { get; set; }
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
        }
    }

    internal class MessageMap<T>
    {
        private List<FieldMap> _fields = new List<FieldMap>();

        protected FieldMap Field<TField>(Expression<Func<T, TField>> expression)
        {
            MemberExpression memberExpression = GetMemberExpression(expression);

            FieldMap fieldMap = new FieldMap(memberExpression);

            _fields.Add(fieldMap);

            return fieldMap;
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
    }

    internal class FieldMap
    {
        private readonly MemberExpression _expression;
        private bool _required;

        public FieldMap(MemberExpression expression)
        {
            _expression = expression;
        }

        public FieldMap Required()
        {
            _required = true;
            return this;
        }
    }
}
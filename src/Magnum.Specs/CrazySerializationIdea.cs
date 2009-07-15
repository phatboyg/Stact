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
namespace Magnum.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;
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

        	SetVersion(2);

            Field(x => x.Birthdate).Number(7);
        }
    }

    internal class MessageMap<T>
    {
        private List<FieldMap<T>> _fields = new List<FieldMap<T>>();
    	private int _version = 1;

    	protected FieldMap<T> Field<TField>(Expression<Func<T, TField>> expression)
        {
            MemberExpression memberExpression = GetMemberExpression(expression);

            FieldMap<T> fieldMap = new FieldMap<T>(this, memberExpression, _version, _fields.Count + 1);

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

        public void Validate()
        {
            foreach (FieldMap<T> field in _fields)
            {
                field.Validate();
            }
        }

    	protected int Version
    	{
			get { return _version; }
    	}

		protected void SetVersion(int version)
		{
			if (version < _version)
				throw new ArgumentException("Versions must be incremental");

			_version = version;
		}

        public string GenerateProtoFile()
        {
            StringBuilder sb=new StringBuilder();
            sb.AppendFormat("message {0} {{{1}", typeof(T).Name, Environment.NewLine);
            int i = 0;
            foreach (FieldMap<T> map in _fields)
            {
                sb.AppendFormat("    {0}{1}", GenerateRules(map, ++i), Environment.NewLine);
            }
            sb.AppendLine("}");

            return sb.ToString();
        }
        private string GenerateRules(FieldMap<T> map, int numberTag)
        {
            StringBuilder sb = new StringBuilder();
            if(map.IsRequired)
            {
                sb.Append("required ");
            }
            else
            {
                sb.Append("optional ");
            }

            sb.Append("type ");
            sb.Append("name ");
            sb.AppendFormat("= {0} ", numberTag);
            
            if(map.DefaultValue != null)
            {
                sb.AppendFormat("[default = {0}]", map.DefaultValue);
            }

            sb.AppendLine();

            return sb.ToString();
        }
    }

    internal class FieldMap<T>
    {
        private readonly MessageMap<T> _map;
        private readonly MemberExpression _expression;
        private bool _isRequired;
        private bool _isRepeated;
        private string _defaultValue;
        private int _number;

        public FieldMap(MessageMap<T> map, MemberExpression expression, int version, int number)
        {
            _map = map;
            _expression = expression;
            _number = number;
        }

        public FieldMap<T> Required()
        {
            _isRequired = true;
            return this;
        }
        public FieldMap<T> SetDefaultValue(string value)
        {
            _defaultValue = value;
            return this;
        }

        public bool IsRequired
        {
            get { return _isRequired; }
        }

        public bool IsRepeated
        {
            get { return _isRepeated; }
        }

        public string DefaultValue
        {
            get { return _defaultValue; }
        }

        public void Validate()
        {
            Type propertyType = ((PropertyInfo) _expression.Member).PropertyType;

            if (propertyType.IsValueType)
            {
                if (_isRequired == false)
                {
                    if (!propertyType.IsGenericType || propertyType.GetGenericTypeDefinition() != typeof (Nullable<>))
                    {
                        throw new FieldValidationException(propertyType, "Value types must be nullable if not required: " + _expression.Member.Name);
                    }
                }
            }
        }

        public FieldMap<T> Number(int number)
        {
            _number = number;
            return this;
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
namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    public class MessageDescriptorFactory
    {
        private Dictionary<Type, IMessageDescriptor> _things = new Dictionary<Type, IMessageDescriptor>();

        public IMessageDescriptor<TMessage> Build<TMessage>(MessageMap<TMessage> map) where TMessage : class, new()
        {
            if (_things.ContainsKey(typeof(TMessage)))
                return (IMessageDescriptor<TMessage>)_things[typeof(TMessage)];

            var desc = new MessageDescriptor<TMessage>();

            foreach (var field in map.Fields)
            {
                var wireType = DetermineWireType(field.FieldType);
                var tag = field.NumberTag;
                var readFunc = GetReader(field.Lambda);
                var writeFunc = GetWriter(field.Lambda);

                desc.AddWriter(tag, wireType, writeFunc);
                desc.AddReader(tag, wireType, readFunc);
            }

            _things.Add(typeof(TMessage), desc);
            return desc;
        }

        private WireType DetermineWireType(Type type)
        {
            return WireType.Varint;
        }

        private Func<T, TProperty> GetReader<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var body = expression.Body as MemberExpression;

            if (body.Member.MemberType != MemberTypes.Property)
                throw new ArgumentException("Not a property: " + body.Member.Name);

            Type t = body.Member.DeclaringType;
            PropertyInfo prop = t.GetProperty(body.Member.Name,
                                              BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            ParameterExpression instance = Expression.Parameter(typeof(T), "instance");
            var property =
                Expression.Lambda<Func<T, TProperty>>(Expression.Call(instance, prop.GetGetMethod()), instance)
                    .Compile();


            return property;
        }

        private Action<T, TProperty> GetWriter<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var body = expression.Body as MemberExpression;

            if (body.Member.MemberType != MemberTypes.Property)
                throw new ArgumentException("Not a property: " + body.Member.Name);

            Type t = body.Member.DeclaringType;
            PropertyInfo prop = t.GetProperty(body.Member.Name,
                                              BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            ParameterExpression instance = Expression.Parameter(typeof(T), "instance");
            ParameterExpression value = Expression.Parameter(typeof(TProperty), "value");

            // value as T is slightly faster than (T)value, so if it's not a value type, use that
            UnaryExpression instanceCast = (!prop.DeclaringType.IsValueType)
                                               ? Expression.TypeAs(instance, prop.DeclaringType)
                                               : Expression.Convert(instance, prop.DeclaringType);
            UnaryExpression valueCast = (!prop.PropertyType.IsValueType)
                                            ? Expression.TypeAs(value, prop.PropertyType)
                                            : Expression.Convert(value, prop.PropertyType);

            var property =
                Expression.Lambda<Action<T, TProperty>>(
                        Expression.Call(instanceCast, prop.GetSetMethod(), valueCast), new[] { instance, value }).Compile();

            


            return property;
        }
    }
}
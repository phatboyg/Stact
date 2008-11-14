namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Common.Reflection;

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

                var fp = new FastProperty<TMessage>(field.PropertyInfo);

                desc.AddProperty(tag, wireType, fp);
            }

            _things.Add(typeof(TMessage), desc);
            return desc;
        }

        private WireType DetermineWireType(Type type)
        {
            if (type.IsEnum)
                return WireType.Varint;

            if (typeof(DateTime).Equals(type)) //uint64
                return WireType.Varint;

            if (typeof(Guid).Equals(type)) //two uint64
                return WireType.Varint;

            if (typeof(int).Equals(type))
                return WireType.Varint;

            

            return WireType.LengthDelimited;
        }

        private static Func<T, TProperty> GetReader<T, TProperty>(Expression<Func<T, TProperty>> expression)
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

        private static Action<T, TProperty> GetWriter<T, TProperty>(Expression<Func<T, TProperty>> expression)
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
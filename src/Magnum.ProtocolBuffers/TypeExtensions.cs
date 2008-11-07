namespace Magnum.ProtocolBuffers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public static class TypeExtensions
    {
        public static bool IsRepeatedType(this Type type)
        {
            if (type.IsArray) return true;
            if (typeof(IList).IsAssignableFrom(type)) return true;

            if (type.IsGenericType)
            {
                var genArgs = type.GetGenericArguments();
                var genList = typeof(IList<>).MakeGenericType(genArgs);
                return genList.IsAssignableFrom(type);
            }
            return false;
        }

        public static bool IsRequiredType(this Type type)
        {
            return type.IsValueType && !type.IsGenericType;
        }
        public static bool IsDictionary(this Type type)
        {
            return typeof(IDictionary<,>).IsAssignableFrom(type);
        }
    }
}
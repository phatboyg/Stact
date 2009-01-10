namespace Magnum.ProtocolBuffers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    public static class TypeExtensions
    {
        public static bool IsRepeatedType(this Type type)
        {
            return typeof (IEnumerable).IsAssignableFrom(type);
        }

        public static bool IsRequiredType(this Type type)
        {
            return type.IsValueType && !type.IsGenericType;
        }
        public static bool IsDictionary(this Type type)
        {
            return typeof(IDictionary<,>).IsAssignableFrom(type);
        }

        public static bool IsMessagePrimative(this Type type)
        {
            return type.IsPrimitive || typeof(string).IsAssignableFrom(type);
        }

        public static string ToGoogleTypeName(this Type type)
        {
            if (typeof(int).Equals(type)) return "int32";
            if (typeof(int?).Equals(type)) return "int32";
            if (typeof(string).Equals(type)) return "string";
            return type.Name;
        }



        public static Type GetTypeEnumerated(this Type type)
        {
            Type result = typeof (object);

            if (type.IsArray)
                result = type.GetElementType();

            if (type.IsGenericType)
            {
                if(typeof(IList<>).MakeGenericType(type.GetGenericArguments()[0]).IsAssignableFrom(type))
                {
                    result = type.GetGenericArguments()[0];
                }
            }

            return result;
        }
    }

    public static class StringExtensions
    {
        public static string ToBoxCuttingCase(this string s)
        {
            StringBuilder sb = new StringBuilder();
            var first = true;

            foreach(char c in s)
            {
                if(first)
                {
                    sb.Append(c);
                    first = false;
                    continue;
                }
                
                if(char.IsUpper(c))
                {
                    sb.Append('_');
                }

                sb.Append(c);
            }
            return sb.ToString().ToLower();
        }
    }
}
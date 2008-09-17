using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Magnum.Serialization.Specs
{
    public class Cerealizer
    {
        private readonly ISerializationFormatter _formatter = new XmlFormatter();

        public string MilkIt<T>(T flat)
        {
            Type objectType = typeof (T);

            _formatter.StartObject(objectType);

            PropertyInfo[] properties = objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo propertyInfo in properties)
            {
                Debug.WriteLine(string.Format("Property: {0}", propertyInfo.Name));

                object value = propertyInfo.GetValue(flat, BindingFlags.Instance | BindingFlags.Public, null, null, CultureInfo.InvariantCulture);

                _formatter.SetProperty(propertyInfo.Name, propertyInfo.PropertyType, value);

            }


            string result =  _formatter.GetString();

            Debug.WriteLine(result);
            return result;
        }
    }
}
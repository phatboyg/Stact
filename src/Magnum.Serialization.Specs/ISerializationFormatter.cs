using System;

namespace Magnum.Serialization.Specs
{
    public interface ISerializationFormatter
    {
        void StartObject(Type objectType);
        string GetString();
        void SetProperty(string name, Type type, object value);
    }
}
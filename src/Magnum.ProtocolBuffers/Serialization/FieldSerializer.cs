namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using Common.Reflection;
    using Specs;

    public class FieldSerializer
    {
        public FieldRules Rules { get; set;}
        public int FieldTag { get; set; }
        public FastProperty Func { get; set; }
        public Type NetType { get; set; }
        public ISerializationStrategy Strategy { get; set; }

    }
}
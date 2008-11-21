namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using Common.Reflection;
    using Specs;

    public class RepeatedFieldDescriptor<TMessage>
    {
        public FieldRules Rules { get; set; }
        public int FieldTag { get; set; }
        public FastProperty<TMessage> Func { get; set; }
        public void AddItem()
        {
        }
        //public void ReadItem

    

        public WireType WireType { get; set; }
        public Type NetType { get; set; }
    }
}
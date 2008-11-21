namespace Magnum.ProtocolBuffers.Specs.TestMappings
{
    using ProtocolBuffers.Mapping;
    using TestMessages;

    public class Int32MessageMap :
        MessageMap<Int32Message>
    {
        public Int32MessageMap()
        {
            Field(x => x.Value).MakeRequired();
        }
    }
}
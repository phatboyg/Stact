namespace Magnum.ProtocolBuffers.Specs.TestMappings
{
    using TestMessages;

    public class Test3Map :
        MessageMap<Test3>
    {
        public Test3Map()
        {
            Field(m => m.I);
        }
    }
}
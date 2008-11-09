namespace Magnum.ProtocolBuffers.ClrMessages
{
    using System;

    public class GuidMessage :
        MessageMap<Guid>
    {
        public GuidMessage()
        {
            Field(m => m.ToByteArray());
        }
    }
}
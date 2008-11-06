namespace Magnum.ProtocolBuffers
{
    using System;

    public class MessageMap<TMessage>
    {
        private readonly Type _messageType;

        public MessageMap()
        {
            _messageType = typeof (TMessage);
            Name = _messageType.Name;
        }

        public string Name { get; private set; }
    }
}
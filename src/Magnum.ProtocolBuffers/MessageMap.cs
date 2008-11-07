namespace Magnum.ProtocolBuffers
{
    using System;
    using System.Collections.Generic;

    public class MessageMap<TMessage>
    {
        private readonly Type _messageType;
        private readonly IList<FieldMap> _fields;

        public MessageMap()
        {
            _fields = new List<FieldMap>();
            _messageType = typeof (TMessage);
            Name = _messageType.Name;
        }

        public string Name { get; private set; }

        public void AddField(FieldMap map)
        {
            _fields.Add(map);
        }

        public int FieldCount
        {
            get
            {
                return _fields.Count;
            }
        }
    }
}
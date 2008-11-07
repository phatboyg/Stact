namespace Magnum.ProtocolBuffers
{
    using System;
    using System.Collections.Generic;
    using Common;

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
            if(ExtensionRange.Contains(map.NumberTag))
                throw new ProtoMappingException(string.Format("You have tried to map a field with a number tag of {0} in the extention range {1} to {2}",map.NumberTag, ExtensionRange.LowerBound, ExtensionRange.UpperBound));

            _fields.Add(map);
        }

        public int FieldCount
        {
            get
            {
                return _fields.Count;
            }
        }

        public Range<int> ExtensionRange { get; private set; }

        public void SetAsideExtensions(int lower, int upper)
        {
            ExtensionRange = new Range<int>(lower, upper, true,true);
        }
    }
}
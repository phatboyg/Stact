namespace Magnum.ProtocolBuffers.Mapping
{
    using System;
    using System.Collections.Generic;
    using Common;

    public class MessageDescriptor<TMessage> :
        IMessageDescriptor<TMessage>
    {
        private readonly HashSet<int> _numberTagsUsed = new HashSet<int>();
        private readonly List<FieldDescriptor<TMessage>> _fields;

        public MessageDescriptor()
        {
            Name = typeof (TMessage).Name;
            _fields =  new List<FieldDescriptor<TMessage>>();
            ExtensionRange = new Range<int>(0, 0, false, false);
        }

        public Range<int> ExtensionRange { get; private set; }
        public void SetAsideExtensions(int lower, int upper)
        {
            ExtensionRange = new Range<int>(lower, upper, true, true);
        }
        public IList<FieldDescriptor<TMessage>> Fields
        {
            get
            {
                return _fields;
            }
        }

        public int FieldCount
        {
            get { return Fields.Count; }
        }

        public  void AddField(FieldMap<TMessage> map)
        {
            if (ExtensionRange.Contains(map.NumberTag))
                throw new ProtoMappingException(string.Format("You have tried to map a field with a number tag of {0} in the extention range {1} to {2}", map.NumberTag, ExtensionRange.LowerBound, ExtensionRange.UpperBound));

            if (_numberTagsUsed.Contains(map.NumberTag))
                throw new ProtoMappingException(string.Format("NumberTag {0} is already assigned to {1}", map.NumberTag, _fields.Find(o => o.NumberTag == map.NumberTag).Name));

            _numberTagsUsed.Add(map.NumberTag);
            FieldDescriptor<TMessage> desc = new FieldDescriptor<TMessage>();
            Fields.Add(desc);
        }

        public string Name
        {
            get; set;
        }

        public Type TypeMapped
        {
            get { return typeof(TMessage); }
        }
    }
}
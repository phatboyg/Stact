namespace Magnum.ProtocolBuffers.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Common;
    using Internal;

    public class MessageMap<TMessage> :
        IMap<TMessage>
    {
        private readonly Type _messageType;
        private readonly List<FieldMap<TMessage>> _fields;
        private readonly HashSet<int> _tagsUsed;
        private int _currentNumberTag = 0;

        public MessageMap()
        {
            ExtensionRange = new Range<int>(0,0,false,false);

            _fields = new List<FieldMap<TMessage>>();
            _tagsUsed = new HashSet<int>();
            _messageType = typeof (TMessage);
            Name = _messageType.Name;
        }

        public string Name { get; set; }
        public Range<int> ExtensionRange { get; private set; }
        public int FieldCount
        {
            get
            {
                return _fields.Count;
            }
        }
        public int CurrentNumberTag
        {
            get { return _currentNumberTag; }
        }


        public void SetAsideExtensions(int lower, int upper)
        {
            ExtensionRange = new Range<int>(lower, upper, true,true);
        }

        public FieldMap<TMessage> Field(Expression<Func<TMessage, object>> field)
        {
            return Field(field, GetNextNumberTag());
        }
        public FieldMap<TMessage> Field(Expression<Func<TMessage, object>> field, int numberTag)
        {
            RecalibrateNumberTagIfNecessary(numberTag);

            var map = new FieldMap<TMessage>(numberTag, field);
            AddField(map);
            return map;
        }


        void IMap.Visit(IMappingVisitor visitor)
        {
            visitor.CurrentType = typeof (TMessage);
            
            visitor.AddMap(string.Format("message {0} {{", this.Name));
            
            foreach (IMappingPart map in _fields)
            {
                map.Visit(visitor);
            }

            visitor.AddMap("}");
        }

        Type IMap.TypeMapped
        {
            get { return typeof (TMessage); }
        }

        public IList<FieldMap<TMessage>> Fields
        {
            get { return _fields; }
        }

        private int GetNextNumberTag()
        {
            return ++_currentNumberTag;
        }
        private void RecalibrateNumberTagIfNecessary(int numberTag)
        {
            if (!numberTag.Equals(_currentNumberTag))
                _currentNumberTag = ++numberTag;
        }
        private void AddField(FieldMap<TMessage> map)
        {
            if (ExtensionRange.Contains(map.NumberTag))
                throw new ProtoMappingException(string.Format("You have tried to map a field with a number tag of {0} in the extention range {1} to {2}", map.NumberTag, ExtensionRange.LowerBound, ExtensionRange.UpperBound));

            if(_tagsUsed.Contains(map.NumberTag))
                throw new ProtoMappingException(string.Format("NumberTag {0} is already assigned to {1}", map.NumberTag, _fields.Find(o=>o.NumberTag == map.NumberTag).Name));

            _tagsUsed.Add(map.NumberTag);
            _fields.Add(map);
        }
    }
}
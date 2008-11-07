namespace Magnum.ProtocolBuffers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;
    using Common;
    using Internal;

    public class MessageMap<TMessage> :
        IMapping
    {
        private readonly Type _messageType;
        private readonly IList<FieldMap> _fields;
        private int _currentNumberTag = 0;

        public MessageMap()
        {
            ExtensionRange = new Range<int>(0,0,false,false);

            _fields = new List<FieldMap>();
            _messageType = typeof (TMessage);
            Name = _messageType.Name;
        }

        public string Name { get; private set; }

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

        private int GetNextNumberTag()
        {
            return ++_currentNumberTag;
        }

        public void SetAsideExtensions(int lower, int upper)
        {
            ExtensionRange = new Range<int>(lower, upper, true,true);
        }

        public FieldMap Field(Expression<Func<TMessage, object>> field)
        {
            return Field(field, GetNextNumberTag());
        }
        public FieldMap Field(Expression<Func<TMessage, object>> field, int numberTag)
        {
            RecalibrateNumberTagIfNecessary(numberTag);

            var prop = ReflectionHelper.GetProperty(field);
            var map = new FieldMap(prop, numberTag);
            AddField(map);
            return map;
        }

        private void RecalibrateNumberTagIfNecessary(int numberTag)
        {
            if (!numberTag.Equals(_currentNumberTag)) _currentNumberTag = ++numberTag;
        }

        private void AddField(FieldMap map)
        {
            if (ExtensionRange.Contains(map.NumberTag))
                throw new ProtoMappingException(string.Format("You have tried to map a field with a number tag of {0} in the extention range {1} to {2}", map.NumberTag, ExtensionRange.LowerBound, ExtensionRange.UpperBound));

            _fields.Add(map);
        }

        void IMapping.Visit(IMappingVisitor visitor)
        {
            visitor.CurrentType = typeof (TMessage);
            
            visitor.AddMap(string.Format("message {0} {{", this.Name));
            
            foreach (IMapping map in _fields)
            {
                map.Visit(visitor);
            }

            visitor.AddMap("}");
        }
    }
}
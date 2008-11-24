namespace Magnum.ProtocolBuffers
{
    using System;
    using System.Linq.Expressions;
    using Mapping;

    public class MessageMap<TMessage> :
        IMap<TMessage>
    {
        private readonly MessageDescriptor<TMessage> _descriptor;
        private int _currentNumberTag;

        public MessageMap()
        {
            _descriptor = new MessageDescriptor<TMessage>();
        }

        public Type TypeMapped
        {
            get
            {
                return typeof (TMessage);
            }
        }
        public int CurrentNumberTag
        {
            get { return _currentNumberTag; }
        }
        public MessageMap<TMessage> SetAsideExtensions(int lower, int upper)
        {
            _descriptor.SetAsideExtensions(lower, upper);
            return this;
        }
        public FieldMap<TMessage> Field(Expression<Func<TMessage, object>> field)
        {
            return Field(field, GetNextNumberTag());
        }
        public FieldMap<TMessage> Field(Expression<Func<TMessage, object>> field, int numberTag)
        {
            RecalibrateNumberTagIfNecessary(numberTag);

            var map = new FieldMap<TMessage>(numberTag, field);
            _descriptor.AddField(map);
            return map;
        }
        public MessageMap<TMessage> OverrideName(string name)
        {
            _descriptor.Name = name;
            return this;
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

        public MessageDescriptor<TMessage> GetDescriptor()
        {
            return _descriptor;
        }
    }
}
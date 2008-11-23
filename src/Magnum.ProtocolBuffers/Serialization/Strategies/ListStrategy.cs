namespace Magnum.ProtocolBuffers.Serialization.Strategies
{
    using System;
    using System.Collections;
    using Streams;

    public class ListStrategy :
        ISerializationStrategy
    {
        private readonly ISerializationStrategy _subStrategy;

        public ListStrategy(ISerializationStrategy subStrategy)
        {
            _subStrategy = subStrategy;
        }

        public bool CanHandle(Type type)
        {
            return typeof (IEnumerable).IsAssignableFrom(type);
        }

        public void Serialize(CodedOutputStream stream, int fieldNumber, object value)
        {
            IEnumerable collectionItems = (IEnumerable) value;

            foreach (var individualItem in collectionItems)
            {
                _subStrategy.Serialize(stream, fieldNumber, individualItem);
            }
        }

        public object Deserialize(CodedInputStream stream)
        {
            return _subStrategy.Deserialize(stream);
        }

        public WireType WireType
        {
            get { return _subStrategy.WireType; }
        }
    }
}
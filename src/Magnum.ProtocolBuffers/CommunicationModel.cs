namespace Magnum.ProtocolBuffers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Internal;
    using Mapping;
    using Serialization;
    using Serialization.Strategies;

    public class CommunicationModel
    {
        private readonly MessageSerializerFactory _factory;

        private readonly IDictionary<Type, IMessageDescriptor> _descriptors = new Dictionary<Type, IMessageDescriptor>();
        private readonly List<ISerializer> _messageSerializers = new List<ISerializer>();
        private readonly List<ISerializationStrategy> _valueTypeSerializers = new List<ISerializationStrategy>();

        public CommunicationModel()
        {
            AddFieldSerializer(new StringStrategy());
            
            AddFieldSerializer(new SignedInt32Strategy());
            AddFieldSerializer(new UnsignedInt32Strategy());
            AddFieldSerializer(new NullableSignedInt32Strategy());
            
            AddFieldSerializer(new SignedInt64Strategy());
            AddFieldSerializer(new UnsignedInt64Strategy());
            AddFieldSerializer(new NullableSignedInt64Strategy());

            AddFieldSerializer(new BooleanSerialization());

            AddFieldSerializer(new ByteStrategy());
            AddFieldSerializer(new ByteArrayStrategy());

            AddFieldSerializer(new SingleStrategy());
            AddFieldSerializer(new DoubleStrategy());
            

            _factory = new MessageSerializerFactory(this);
        }

        public int NumberOfMessagesMapped
        {
            get { return _descriptors.Count; }
        }

        /// <summary>
        /// This method wipes out all previous stuff and rebuilds the model
        /// </summary>
        /// <param name="builder"></param>
        public void Initialize(Action<CommunicationModelBuilder> builder)
        {
            builder.Invoke(new CommunicationModelBuilder(this));
            BuildSerializers();
        }

        public void WriteMappingsTo(string directory)
        {
            if(!Directory.Exists(directory))
                throw new DirectoryNotFoundException("Didn't find " + directory);
        }
        public ISerializer GetSerializer(Type type)
        {
            return _messageSerializers.Find(s=>s.CanHandle(type));
        }


        //study StructureMap to look at how to handle these methods        
        public void AddMapping<TMessage>(IMap<TMessage> map) where TMessage : class, new()
        {
            IMessageDescriptor descriptor = map.GetDescriptor();
            if(_descriptors.ContainsKey(descriptor.TypeMapped))
                throw new ProtoMappingException(string.Format("You have already added the type {0} to the communication model", map.TypeMapped));

            _descriptors.Add(descriptor.TypeMapped, descriptor);
        }
        internal IMessageDescriptor GetDescriptor(Type descriptorType)
        {
            return _descriptors[descriptorType];
        }
        internal void AddSerializer(ISerializer serializer)
        {
            _messageSerializers.Add(serializer);
        }
        internal void AddFieldSerializer(ISerializationStrategy strategy)
        {
            _valueTypeSerializers.Add(strategy);
        }
        internal void AddDescriptor(IMessageDescriptor descriptor)
        {
            if (_descriptors.ContainsKey(descriptor.TypeMapped))
                throw new ProtoMappingException(string.Format("You have already added the type {0} to the communication model", descriptor.TypeMapped));

            _descriptors.Add(descriptor.TypeMapped, descriptor);
        }
        private void BuildSerializers()
        {
            foreach (var pair in _descriptors)
            {
                AddSerializer(_factory.Build(pair.Value));
            }
        }

        internal ISerializationStrategy GetFieldSerializer(Type type)
        {
           return _valueTypeSerializers.Find(x => x.CanHandle(type));
        }
        public bool HasSerializer(Type mapped)
        {
            return _messageSerializers.Exists(s => s.CanHandle(mapped));
        }
    }
}
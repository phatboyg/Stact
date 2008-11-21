namespace Magnum.ProtocolBuffers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq.Expressions;
    using System.Reflection;
    using Internal;
    using Serialization;

    public class CommunicationModel
    {
        private readonly IDictionary<Type, IMap> _mappings = new Dictionary<Type, IMap>();
        private readonly MessageSerializerFactory _factory = new MessageSerializerFactory();
        private readonly List<IMessageSerializer> _descriptors = new List<IMessageSerializer>();

        
        public int NumberOfMessagesMapped
        {
            get { return _mappings.Count; }
        }

        public void AddMappingsFromAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetExportedTypes())
            {
                if(!type.IsGenericType && typeof(IMap).IsAssignableFrom(type))
                {
                    Type messageType = type.BaseType.GetGenericArguments()[0];
                    var genericArguments = new[] {messageType};
                    var parameter = Expression.Parameter(type.GetInterfaces()[0], "map");
                    var call = Expression.Call(Expression.Parameter(typeof (CommunicationModel), "who_cares"),
                                               "AddMapping", genericArguments, parameter);

                    var mapInstance = Activator.CreateInstance(type);
                    
                    call.Method.Invoke(this, new []{mapInstance});
                }
            }
        }


        public void AddMapping<TMessage>(IMap<TMessage> map) where TMessage : class, new()
        {
            if(_mappings.ContainsKey(map.TypeMapped))
                throw new ProtoMappingException(string.Format("You have already added the type {0} to the communication model", map.TypeMapped));

            _mappings.Add(map.TypeMapped, map);
            _descriptors.Add(_factory.Build(map));
        }









        public void WriteMappingsTo(string directory)
        {
            if(!Directory.Exists(directory))
                throw new DirectoryNotFoundException("Didn't find " + directory);
        }


        public void Validate()
        {
            
        }
    }
}
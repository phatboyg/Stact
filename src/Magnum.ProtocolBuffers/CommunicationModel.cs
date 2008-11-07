namespace Magnum.ProtocolBuffers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Internal;

    public class CommunicationModel
    {
        private IDictionary<Type, IMapping> _mappings = new Dictionary<Type, IMapping>();

        public int NumberOfMessagesMapped
        {
            get { return _mappings.Count; }
        }

        public void AddMappingsFromAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetExportedTypes())
            {
                if (!type.IsGenericType && typeof(IMapping).IsAssignableFrom(type))
                {
                    IMapping mapping = (IMapping)Activator.CreateInstance(type);
                    AddMapping(mapping);
                }
            }
        }

        public void WriteMappingsTo(string directory)
        {
            if(!Directory.Exists(directory)) throw new DirectoryNotFoundException("Didn't find " + directory);


        }

        private void AddMapping(IMapping mapping)
        {
            if(_mappings.ContainsKey(mapping.TypeMapped))
                throw new ProtoMappingException(string.Format("You have already added the type {0} to the communication model", mapping.TypeMapped));

            _mappings.Add(mapping.TypeMapped, mapping);
        }

        public void Validate()
        {
            
        }
    }
}
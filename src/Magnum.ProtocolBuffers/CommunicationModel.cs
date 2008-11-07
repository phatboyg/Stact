namespace Magnum.ProtocolBuffers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Internal;

    public class CommunicationModel
    {
        private IList<IMapping> _mappings = new List<IMapping>();

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
            _mappings.Add(mapping);
        }
    }
}
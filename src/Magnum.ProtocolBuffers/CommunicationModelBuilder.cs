namespace Magnum.ProtocolBuffers
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Mapping;

    public class CommunicationModelBuilder
    {
        private readonly CommunicationModel _model;

        public CommunicationModelBuilder(CommunicationModel model)
        {
            _model = model;
        }

        public void AddMappingsFromAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetExportedTypes())
            {
                if (!type.IsGenericType && typeof(IMap).IsAssignableFrom(type))
                {
                    Type messageType = type.BaseType.GetGenericArguments()[0];
                    var genericArguments = new[] { messageType };
                    var inter = type.GetInterfaces()[0];
                    var parameter = Expression.Parameter(inter, "map");
                    var call = Expression.Call(Expression.Parameter(typeof(CommunicationModel), "who_cares"),
                                               "AddMapping", genericArguments, parameter);

                    var mapInstance = Activator.CreateInstance(type);

                    call.Method.Invoke(_model, new[] { mapInstance });
                }
            }
        }
        public void AddMapping<TMessage>(IMap<TMessage> map) where TMessage : class, new()
        {
            IMessageDescriptor descriptor = map.GetDescriptor();
            _model.AddDescriptor(descriptor);
        }
    }
}
namespace Magnum.ProtocolBuffers.Serialization.FieldManipulationStrategies
{
    public class InstanceManipulationStrategy
    {
        
    }

    public interface IManipulationStrategy
    {
        /// <summary>
        /// On a list item this would be an add
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        void Set(object instance, object value);
        //object Get(object instance);
    }
}
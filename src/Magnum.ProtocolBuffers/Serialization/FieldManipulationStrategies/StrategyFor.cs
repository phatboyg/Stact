namespace Magnum.ProtocolBuffers.Serialization.FieldManipulationStrategies
{
    using System;

    public static class StrategyFor
    {
        public static IFieldStrategy Get(Type fieldReturnType)
        {
            if((fieldReturnType).IsMessagePrimative())
            {
                return new PrimativeFieldStrategy();
            }
            else if((fieldReturnType).IsRepeatedType())
            {
                return new RepeatableFieldStrategy();
            }

            return new InstanceFieldStrategy();
        }
    }
}
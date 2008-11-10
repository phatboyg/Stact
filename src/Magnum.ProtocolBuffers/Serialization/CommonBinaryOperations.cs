namespace Magnum.ProtocolBuffers.Serialization
{
    public static class CommonBinaryOperations
    {
        public static bool HasMostSignificantBitSet(this byte data)
        {
            return (data & 0x80) == 0x80;
        }

        public static byte RemoveMsb(this byte data)
        {
            return (byte)(data & 0x7f);
        }
    }
}
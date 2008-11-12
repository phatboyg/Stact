namespace Magnum.ProtocolBuffers.Serialization
{
    using System;

    public static class CommonBinaryOperations
    {
        public static bool IsMsbUnset(this byte data)
        {
            return (data & 0x80) == 0x80;
        }

        public static bool IsMsbUnset(this int data)
        {
            return (data & 0x80) == 0x80;
        }

        public static byte RemoveMsb(this byte data)
        {
            return (byte)(data & 0x7f);
        }
        public static UInt64 RemoveMsb(this int data)
        {
            return (byte)(data & 0x7f);
        }
    }
}
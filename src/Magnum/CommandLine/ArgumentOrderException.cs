namespace Magnum.CommandLine
{
    using System;
    using System.Runtime.Serialization;

    public class ArgumentOrderException : Exception
    {
        public ArgumentOrderException()
        {
        }

        public ArgumentOrderException(string message) : base(message)
        {
        }

        public ArgumentOrderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ArgumentOrderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
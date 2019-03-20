namespace Core
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class UnexpectedEventException : Exception
    {
        public UnexpectedEventException()
        {
        }

        public UnexpectedEventException(string message)
            : base(message)
        {
        }

        public UnexpectedEventException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected UnexpectedEventException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
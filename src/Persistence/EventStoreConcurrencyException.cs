namespace Persistence
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class EventStoreConcurrencyException : Exception
    {
        public EventStoreConcurrencyException()
        {
        }

        public EventStoreConcurrencyException(string message)
            : base(message)
        {
        }

        public EventStoreConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected EventStoreConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
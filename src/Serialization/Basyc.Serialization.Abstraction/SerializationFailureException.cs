namespace Basyc.Serialization.Abstraction
{
    public class SerializationFailureException : Exception
    {
        public SerializationFailureException(string message) : base(message)
        {

        }

        public SerializationFailureException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
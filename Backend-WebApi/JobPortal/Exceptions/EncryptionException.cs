using System;

namespace JobPortal.Exceptions
{
    public class EncryptionException : Exception
    {
        public EncryptionException(string message)
            : base(message) { }

        public EncryptionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}

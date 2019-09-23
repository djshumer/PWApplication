using System;

namespace PWApplication.TransactionApi.Infrastructure.Exceptions
{
    /// <summary>
    /// Exception type for app exceptions
    /// </summary>
    public class TransactionDomainException : Exception
    {
        public TransactionDomainException()
        { }

        public TransactionDomainException(string message)
            : base(message)
        { }

        public TransactionDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transaction.Api.Infrastructure.Exceptions
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

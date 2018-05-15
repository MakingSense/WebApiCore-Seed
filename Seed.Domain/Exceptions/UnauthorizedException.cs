using System;

namespace Seed.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when the current role/user does not satisfy authorization requirements
    /// </summary>
    public class UnauthorizedException : Exception
    {
        /// <summary> Initializes a new instance of the <see cref="UnauthorizedException"/> class. </summary>
        public UnauthorizedException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class
        /// </summary>
        /// <param name="message"> Descriptive message for the error </param>
        public UnauthorizedException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class
        /// </summary>
        /// <param name="message"> Descriptive message for the error </param>
        /// <param name="innerException"> The exception that is the cause of the current exception, or a null reference if no inner exception is specified </param>
        public UnauthorizedException(string message, Exception innerException) : base(message, innerException) { }
    }
}

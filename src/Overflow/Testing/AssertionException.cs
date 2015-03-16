using System;

namespace Overflow.Testing
{
    /// <summary>
    /// An exception which is thrown to indicate that an assertion was not met
    /// during a test.
    /// </summary>
    public class AssertionException : Exception
    {
        /// <summary>
        /// Create new AssertionException with a specific message.
        /// </summary>
        /// <param name="message">The Exception message property</param>
        public AssertionException(string message) : base(message) { }
    }
}

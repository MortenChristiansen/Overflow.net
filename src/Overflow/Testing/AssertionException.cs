using System;

namespace Overflow.Testing
{
    public class AssertionException : Exception
    {
        public AssertionException(string message) : base(message) { }
    }
}

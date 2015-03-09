using Overflow.Testing;
using Xunit;

namespace Overflow.Test.Testing
{
    public class AssertionExceptionTests
    {
        [Fact]
        public void Creating_am_assertion_exception_sets_the_message()
        {
            var sut = new AssertionException("message");

            Assert.Equal("message", sut.Message);
        }
    }
}

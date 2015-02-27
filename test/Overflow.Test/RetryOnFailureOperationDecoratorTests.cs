using System;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class RetryOnFailureOperationDecoratorTests
    {
        [Fact]
        public void Exceptions_during_the_execution_of_the_decorated_operation_are_not_propagated()
        {
            var operation = new FakeOperation { ThrowOnExecute = new Exception() };
            var sut = new RetryOnFailureOperationDecorator(operation);

            sut.Execute();
        }
    }
}

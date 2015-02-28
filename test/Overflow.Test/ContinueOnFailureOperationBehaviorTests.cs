using System;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class ContinueOnFailureOperationBehaviorTests
    {
        [Fact]
        public void Exceptions_during_the_execution_of_the_decorated_operation_are_not_propagated()
        {
            var operation = new FakeOperation { ThrowOnExecute = new Exception() };
            var sut = new ContinueOnFailureOperationBehavior(operation);

            sut.Execute();
        }
    }
}

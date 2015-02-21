using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class OperationTests
    {
        [Fact]
        public void Executing_an_operation_calls_the_OnExecute_method()
        {
            var sut = new FakeOperation();

            sut.Execute();
            
            Assert.True(sut.HasExecuted);
        }
    }
}

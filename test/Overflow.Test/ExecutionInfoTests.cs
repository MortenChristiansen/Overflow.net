using System;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class ExecutionInfoTests
    {
        [Fact]
        public void Creating_an_execution_info_populated_properties()
        {
            var operation = new FakeOperation();
            var error = new Exception();
            var started = new DateTimeOffset();
            var completed = new DateTimeOffset();

            var sut = new ExecutionInfo(operation, error, started, completed);

            Assert.Equal(operation, sut.Operation);
            Assert.Equal(error, sut.Error);
            Assert.Equal(started, sut.Started);
            Assert.Equal(completed, sut.Completed);
        }

        [Fact]
        public void You_cannot_create_an_execution_info_without_an_operation()
        {
            Assert.Throws<ArgumentNullException>(() => new ExecutionInfo(null, new Exception(), new DateTimeOffset(), new DateTimeOffset()));
        }
    }
}

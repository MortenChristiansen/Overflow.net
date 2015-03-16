using System;
using Overflow.Test.TestingInfrastructure;
using Xunit;
using Xunit.Extensions;

namespace Overflow.Test
{
    public class ExecutionInfoTests
    {
        [Theory, AutoMoqData]
        public void Creating_an_execution_info_populated_properties(IOperation operation, Exception error, DateTimeOffset started, DateTimeOffset completed)
        {
            var sut = new ExecutionInfo(operation, error, started, completed);

            Assert.Equal(operation, sut.Operation);
            Assert.Equal(error, sut.Error);
            Assert.Equal(started, sut.Started);
            Assert.Equal(completed, sut.Completed);
        }

        [Theory, AutoMoqData]
        public void You_cannot_create_an_execution_info_without_an_operation(Exception error, DateTimeOffset started, DateTimeOffset completed)
        {
            Assert.Throws<ArgumentNullException>(() => new ExecutionInfo(null, error, started, completed));
        }
    }
}

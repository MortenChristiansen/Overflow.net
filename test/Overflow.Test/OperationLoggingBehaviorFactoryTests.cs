using System;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class OperationLoggingBehaviorFactoryTests
    {
        [Fact]
        public void The_logging_behavior_is_created_when_the_log_is_defined_on_the_configuration()
        {
            var sut = new OperationLoggingBehaviorFactory();
            var configuration = new FakeWorkflowConfiguration { Logger = new FakeWorkflowLogger() };
            var operation = new FakeOperation();

            var result = sut.CreateBehaviors(operation, configuration);

            Assert.Equal(1, result.Count);
            Assert.IsType<OperationLoggingBehavior>(result[0]);
        }

        [Fact]
        public void The_logging_behavior_is_not_created_when_the_configuration_has_not_defined_a_logger_to_use()
        {
            var sut = new OperationLoggingBehaviorFactory();
            var configuration = new FakeWorkflowConfiguration();
            var operation = new FakeOperation();

            var result = sut.CreateBehaviors(operation, configuration);

            Assert.Equal(0, result.Count);
        }

        [Fact]
        public void You_cannot_create_the_behavior_without_an_operation()
        {
            var sut = new OperationLoggingBehaviorFactory();

            Assert.Throws<ArgumentNullException>(() => sut.CreateBehaviors(null, new FakeWorkflowConfiguration()));
        }

        [Fact]
        public void You_cannot_create_the_behavior_without_a_configuration()
        {
            var sut = new OperationLoggingBehaviorFactory();

            Assert.Throws<ArgumentNullException>(() => sut.CreateBehaviors(new FakeOperation(), null));
        }
    }
}

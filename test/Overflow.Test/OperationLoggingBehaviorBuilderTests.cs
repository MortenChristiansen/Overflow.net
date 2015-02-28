using System;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class OperationLoggingBehaviorBuilderTests
    {
        [Fact]
        public void The_logging_behavior_is_added_when_the_log_is_defined_on_the_configuration()
        {
            var sut = new OperationLoggingBehaviorBuilder();
            var configuration = new WorkflowConfiguration { Logger = new FakeWorkflowLogger() };
            var operation = new FakeOperation();

            var result = sut.ApplyBehavior(operation, configuration);

            Assert.IsType<OperationLoggingBehavior>(result);
            Assert.IsType<FakeOperation>((result as OperationBehavior).InnerOperation);
        }

        [Fact]
        public void The_logging_behavior_is_not_added_when_the_configuration_has_not_defined_a_logger_to_use()
        {
            var sut = new OperationLoggingBehaviorBuilder();
            var configuration = new WorkflowConfiguration();
            var operation = new FakeOperation();

            var result = sut.ApplyBehavior(operation, configuration);

            Assert.Equal(operation, result);
        }

        [Fact]
        public void You_cannot_apply_behavior_without_an_operation()
        {
            var sut = new OperationLoggingBehaviorBuilder();

            Assert.Throws<ArgumentNullException>(() => sut.ApplyBehavior(null, new WorkflowConfiguration()));
        }

        [Fact]
        public void You_cannot_apply_behavior_without_a_configuration()
        {
            var sut = new OperationLoggingBehaviorBuilder();

            Assert.Throws<ArgumentNullException>(() => sut.ApplyBehavior(new FakeOperation(), null));
        }
    }
}

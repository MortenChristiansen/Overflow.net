using System;
using Overflow.Extensibility;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;

namespace Overflow.Test
{
    public class LogMultiplexerTests : TestBase
    {
        [Fact]
        public void Guards_are_verified()
        {
            VerifyGuards<LogMultiplexer>();
        }

        [Fact]
        public void You_can_create_a_log_multiplexer()
        {
            new LogMultiplexer();
        }

        [Theory, AutoMoqData]
        public void Started_operations_are_forwarded_to_every_supplied_loggers(FakeWorkflowLogger log1, FakeWorkflowLogger log2, IOperation operation)
        {
            var sut = new LogMultiplexer(log1, log2);

            sut.OperationStarted(operation);

            Assert.Equal(1, log1.StartedOperations.Count);
            Assert.Same(operation, log1.StartedOperations[0]);
            Assert.Equal(1, log2.StartedOperations.Count);
            Assert.Same(operation, log2.StartedOperations[0]);
        }

        [Theory, AutoMoqData]
        public void Finished_operations_are_forwarded_to_every_supplied_loggers(FakeWorkflowLogger log1, FakeWorkflowLogger log2, IOperation operation)
        {
            var sut = new LogMultiplexer(log1, log2);

            sut.OperationFinished(operation, TimeSpan.FromSeconds(1));

            Assert.Equal(1, log1.FinishedOperations.Count);
            Assert.Same(operation, log1.FinishedOperations[0]);
            Assert.Equal(TimeSpan.FromSeconds(1), log1.FinishedOperationDurations[0]);
            Assert.Equal(1, log2.FinishedOperations.Count);
            Assert.Same(operation, log2.FinishedOperations[0]);
            Assert.Equal(TimeSpan.FromSeconds(1), log2.FinishedOperationDurations[0]);
        }

        [Theory, AutoMoqData]
        public void Failed_operations_are_forwarded_to_every_supplied_loggers(FakeWorkflowLogger log1, FakeWorkflowLogger log2, IOperation operation, Exception error)
        {
            var sut = new LogMultiplexer(log1, log2);

            sut.OperationFailed(operation, error);

            Assert.Equal(1, log1.OperationFailures.Count);
            Assert.Same(error, log1.OperationFailures[operation][0]);
            Assert.Equal(1, log2.OperationFailures.Count);
            Assert.Same(error, log2.OperationFailures[operation][0]);
        }

        [Theory, AutoMoqData]
        public void Applied_behaviors_are_forwarded_to_every_supplied_loggers(FakeWorkflowLogger log1, FakeWorkflowLogger log2, IOperation operation, OperationBehavior behavior, string description)
        {
            var sut = new LogMultiplexer(log1, log2);

            sut.BehaviorWasApplied(operation, behavior, description);

            Assert.Equal(1, log1.AppliedBehaviors.Count);
            Assert.Same(behavior, log1.AppliedBehaviors[0].Behavior);
            Assert.Equal(description, log1.AppliedBehaviors[0].Description);
            Assert.Same(operation, log1.AppliedBehaviors[0].Operation);
            Assert.Equal(1, log2.AppliedBehaviors.Count);
            Assert.Same(behavior, log2.AppliedBehaviors[0].Behavior);
            Assert.Equal(description, log2.AppliedBehaviors[0].Description);
            Assert.Same(operation, log2.AppliedBehaviors[0].Operation);
        }
    }
}

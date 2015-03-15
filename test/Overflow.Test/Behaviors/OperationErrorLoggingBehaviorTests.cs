using System;
using Overflow.Behaviors;
using Overflow.Extensibility;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test.Behaviors
{
    public class OperationErrorLoggingBehaviorTests
    {
        [Fact]
        public void The_behavior_has_logging_level_precedence()
        {
            var sut = new OperationErrorLoggingBehavior(new FakeWorkflowLogger());

            Assert.Equal(BehaviorPrecedence.PreRecovery, sut.Precedence);
        }

        [Fact]
        public void Exceptions_thrown_during_the_execution_are_logged()
        {
            var innerOperation = new FakeOperation { ThrowOnExecute = new Exception() };
            var logger = new FakeWorkflowLogger();
            var sut = new OperationErrorLoggingBehavior(logger).Attach(innerOperation);

            try { sut.Execute(); }
            catch { }

            Assert.Equal(1, logger.OperationFailures.Count);
            Assert.Equal(1, logger.OperationFailures[innerOperation].Count);
            Assert.Equal(innerOperation.ThrowOnExecute, logger.OperationFailures[innerOperation][0]);
        }

        [Fact]
        public void The_innermost_operation_is_logged_as_the_source()
        {
            var innerOperation = new FakeOperation { ThrowOnExecute = new Exception() };
            var behavior = new FakeOperationBehavior().Attach(innerOperation);
            var logger = new FakeWorkflowLogger();
            var sut = new OperationErrorLoggingBehavior(logger).Attach(behavior);

            try { sut.Execute(); }
            catch { }

            Assert.Equal(1, logger.OperationFailures[innerOperation].Count);
        }

        [Fact]
        public void Logged_exceptions_are_rethrown()
        {
            var innerOperation = new FakeOperation { ThrowOnExecute = new Exception() };
            var sut = new OperationErrorLoggingBehavior(new FakeWorkflowLogger()).Attach(innerOperation);

            Assert.Throws<Exception>(() => sut.Execute());
        }
    }
}

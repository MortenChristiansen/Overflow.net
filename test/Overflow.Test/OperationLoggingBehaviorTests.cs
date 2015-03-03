using System;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class OperationLoggingBehaviorTests
    {
        [Fact]
        public void Executing_the_behavior_logs_the_start_and_end_of_the_operation()
        {
            var innerOperation = new FakeOperation();
            var logger = new FakeWorkflowLogger();
            var sut = new OperationLoggingBehavior(logger).Attach(innerOperation);

            sut.Execute();

            Assert.Equal(1, logger.StartedOperations.Count);
            Assert.Equal(innerOperation, logger.StartedOperations[0]);
            Assert.Equal(1, logger.FinishedOperations.Count);
            Assert.Equal(innerOperation, logger.FinishedOperations[0]);
        }

        [Fact]
        public void The_innermost_operation_is_logged()
        {
            var innerOperation = new FakeOperation();
            var logger = new FakeWorkflowLogger();
            var sut = new OperationLoggingBehavior(logger).Attach(new FakeOperationBehavior().Attach(innerOperation));

            sut.Execute();

            Assert.Equal(innerOperation, logger.StartedOperations[0]);
            Assert.Equal(innerOperation, logger.FinishedOperations[0]);
        }

        [Fact]
        public void Exceptions_thrown_during_the_execution_are_logged()
        {
            var innerOperation = new FakeOperation { ThrowOnExecute = new Exception() };
            var logger = new FakeWorkflowLogger();
            var sut = new OperationLoggingBehavior(logger).Attach(innerOperation);

            try { sut.Execute(); }
            catch { }

            Assert.Equal(1, logger.OperationFailures.Count);
            Assert.Equal(1, logger.OperationFailures[innerOperation].Count);
            Assert.Equal(innerOperation.ThrowOnExecute, logger.OperationFailures[innerOperation][0]);
        }

        [Fact]
        public void Logged_exceptions_are_rethrown()
        {
            var innerOperation = new FakeOperation { ThrowOnExecute = new Exception() };
            var sut = new OperationLoggingBehavior(new FakeWorkflowLogger()).Attach(innerOperation);

            Assert.Throws<Exception>(() => sut.Execute());
        }

        [Fact]
        public void Start_and_finish_are_logged_in_case_of_failure()
        {
            var innerOperation = new FakeOperation { ThrowOnExecute = new Exception() };
            var logger = new FakeWorkflowLogger();
            var sut = new OperationLoggingBehavior(logger).Attach(innerOperation);

            try { sut.Execute(); }
            catch { }

            Assert.Equal(1, logger.StartedOperations.Count);
            Assert.Equal(1, logger.FinishedOperations.Count);
        }
    }
}

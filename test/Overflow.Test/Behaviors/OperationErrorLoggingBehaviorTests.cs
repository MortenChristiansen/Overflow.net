using System;
using Overflow.Behaviors;
using Overflow.Extensibility;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;
using Xunit.Extensions;

namespace Overflow.Test.Behaviors
{
    public class OperationErrorLoggingBehaviorTests : TestBase
    {
        [Theory, AutoMoqData]
        public void The_behavior_has_logging_level_precedence(IWorkflowLogger logger)
        {
            var sut = new OperationErrorLoggingBehavior(logger);

            Assert.Equal(BehaviorPrecedence.PreRecovery, sut.Precedence);
        }

        [Theory, AutoMoqData]
        public void Exceptions_thrown_during_the_execution_are_logged(FakeWorkflowLogger logger, Exception error)
        {
            var innerOperation = new FakeOperation { ThrowOnExecute = error };
            var sut = new OperationErrorLoggingBehavior(logger).Attach(innerOperation);

            try { sut.Execute(); }
            catch { }

            Assert.Equal(1, logger.OperationFailures.Count);
            Assert.Equal(1, logger.OperationFailures[innerOperation].Count);
            Assert.Equal(innerOperation.ThrowOnExecute, logger.OperationFailures[innerOperation][0]);
        }

        [Theory, AutoMoqData]
        public void The_innermost_operation_is_logged_as_the_source(FakeWorkflowLogger logger, Exception error)
        {
            var innerOperation = new FakeOperation { ThrowOnExecute = error };
            var behavior = new FakeOperationBehavior().Attach(innerOperation);
            var sut = new OperationErrorLoggingBehavior(logger).Attach(behavior);

            try { sut.Execute(); }
            catch { }

            Assert.Equal(1, logger.OperationFailures[innerOperation].Count);
        }

        [Theory, AutoMoqData]
        public void Logged_exceptions_are_rethrown(IWorkflowLogger logger, Exception error)
        {
            var innerOperation = new FakeOperation { ThrowOnExecute = error };
            var sut = new OperationErrorLoggingBehavior(logger).Attach(innerOperation);

            Assert.Throws<Exception>(() => sut.Execute());
        }
    }
}

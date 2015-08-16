using System;
using Overflow.Behaviors;
using Overflow.Extensibility;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Overflow.Utilities;
using Xunit;

namespace Overflow.Test.Behaviors
{
    public class OperationExecutionLoggingBehaviorTests : TestBase
    {
        [Theory, AutoMoqData]
        public void The_behavior_has_logging_level_precedence(IWorkflowLogger logger)
        {
            var sut = new OperationExecutionLoggingBehavior(logger);

            Assert.Equal(BehaviorPrecedence.Logging, sut.Precedence);
        }

        [Theory, AutoMoqData]
        public void Executing_the_behavior_logs_the_start_and_end_of_the_operation(FakeWorkflowLogger logger, IOperation innerOperation)
        {
            var sut = new OperationExecutionLoggingBehavior(logger).AttachTo(innerOperation);

            sut.Execute();

            Assert.Equal(1, logger.StartedOperations.Count);
            Assert.Equal(innerOperation, logger.StartedOperations[0]);
            Assert.Equal(1, logger.FinishedOperations.Count);
            Assert.Equal(innerOperation, logger.FinishedOperations[0]);
        }

        [Theory, AutoMoqData]
        public void The_innermost_operation_is_logged(FakeWorkflowLogger logger, IOperation innerOperation)
        {
            var sut = new OperationExecutionLoggingBehavior(logger).AttachTo(new FakeOperationBehavior().AttachTo(innerOperation));

            sut.Execute();

            Assert.Equal(innerOperation, logger.StartedOperations[0]);
            Assert.Equal(innerOperation, logger.FinishedOperations[0]);
        }

        [Theory, AutoMoqData]
        public void Start_and_finish_are_logged_in_case_of_failure(FakeWorkflowLogger logger)
        {
            var innerOperation = new FakeOperation { ThrowOnExecute = new Exception() };
            var sut = new OperationExecutionLoggingBehavior(logger).AttachTo(innerOperation);

            ExecuteIgnoringErrors(sut.Execute);

            Assert.Equal(1, logger.StartedOperations.Count);
            Assert.Equal(1, logger.FinishedOperations.Count);
        }

        [Theory, AutoMoqData]
        public void Executing_an_operation_logs_the_duration(FakeWorkflowLogger logger)
        {
            Time.Stop();
            var innerOperation = new FakeOperation { ExecuteAction = () => Time.Wait(TimeSpan.FromMilliseconds(10)) };
            var sut = new OperationExecutionLoggingBehavior(logger).AttachTo(innerOperation);

            sut.Execute();

            Assert.Equal(TimeSpan.FromMilliseconds(10).TotalMilliseconds, logger.FinishedOperationDurations[0].TotalMilliseconds);
        }
    }
}

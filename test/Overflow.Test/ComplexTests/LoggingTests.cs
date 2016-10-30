using System;
using System.Collections.Generic;
using Overflow.Extensibility;
using Xunit;

namespace Overflow.Test.ComplexTests
{
    public class LoggingTests
    {
        [Fact]
        public void Errors_are_logged_in_the_proper_operation_scope()
        {
            var logger = new TestLogger();
            var sut = Workflow.Configure<OpA>().WithLogger(logger).CreateOperation();

            sut.Execute();

            Assert.Equal(6, logger.Logs.Count);
            Assert.Equal("error", logger.Logs[0]);
            Assert.Equal("finished", logger.Logs[1]);
            Assert.Equal("error", logger.Logs[2]);
            Assert.Equal("finished", logger.Logs[3]);
            Assert.Equal("error", logger.Logs[4]);
            Assert.Equal("finished", logger.Logs[5]);
        }

        class OpA : Operation
        {
            public override IEnumerable<IOperation> GetChildOperations()
            {
                yield return Create<OpB>();
            }
        }

        class OpB : Operation
        {
            public override IEnumerable<IOperation> GetChildOperations()
            {
                yield return Create<OpC>();
            }
        }

        class OpC : Operation
        {
            protected override void OnExecute()
            {
                throw new Exception();
            }
        }

        class TestLogger : IWorkflowLogger
        {
            public List<string> Logs { get; } = new List<string>();

            public void OperationStarted(IOperation operation) {}

            public void OperationFinished(IOperation operation, TimeSpan duration) =>
                Logs.Add("finished");

            public void OperationFailed(IOperation operation, Exception error) =>
                Logs.Add("error");

            public void BehaviorWasApplied(IOperation operation, OperationBehavior behavior, string description) {}
        }
    }
}

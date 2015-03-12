using System;
using Overflow.Behaviors;
using Overflow.Extensibility;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test.Behaviors
{
    public class ContinueOnFailureBehaviorTests
    {
        [Fact]
        public void The_behavior_maintains_workflow_integrity()
        {
            var sut = new ContinueOnFailureBehavior();

            Assert.Equal(BehaviorIntegrityMode.MaintainsWorkflowIntegrity, sut.IntegrityMode);
        }

        [Fact]
        public void Exceptions_during_the_execution_of_the_decorated_operation_are_not_propagated()
        {
            var operation = new FakeOperation { ThrowOnExecute = new Exception() };
            var sut = new ContinueOnFailureBehavior();
            sut.Attach(operation);

            sut.Execute();
        }
    }
}

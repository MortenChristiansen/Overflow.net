using System;
using Overflow.Behaviors;
using Overflow.Extensibility;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;
using Xunit.Extensions;

namespace Overflow.Test.Behaviors
{
    public class ContinueOnFailureBehaviorTests
    {
        [Fact]
        public void The_behavior_has_containment_level_precedence()
        {
            var sut = new ContinueOnFailureBehavior();

            Assert.Equal(BehaviorPrecedence.Containment, sut.Precedence);
        }

        [Fact]
        public void Exceptions_during_the_execution_of_the_decorated_operation_are_not_propagated()
        {
            var operation = new FakeOperation { ThrowOnExecute = new Exception() };
            var sut = new ContinueOnFailureBehavior();
            sut.AttachTo(operation);

            sut.Execute();
        }

        [Theory, AutoMoqData]
        public void Contained_errors_are_logged(Exception error, FakeWorkflowLogger log)
        {
            var sut = new ContinueOnFailureBehavior().AttachTo(new FakeOperation { ThrowOnExecute = error });
            sut.Initialize(new FakeWorkflowConfiguration { Logger = log });

            sut.Execute();

            Assert.Equal(1, log.AppliedBehaviors.Count);
            Assert.Equal("Error swallowed", log.AppliedBehaviors[0].Description);
        }
    }
}

using Overflow.Behaviors;
using Overflow.Extensibility;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;

namespace Overflow.Test.Behaviors
{
    public class ConditionalExecutionBehaviorTests
    {
        [Fact]
        public void The_behavior_has_staging_level_precedence()
        {
            var sut = new ConditionalExecutionBehavior();

            Assert.Equal(BehaviorPrecedence.Staging, sut.Precedence);
        }

        [Fact]
        public void The_inner_operation_is_skipped_when_it_states_that_it_should_be()
        {
            var operation = new SkippableOperation { SkipExecution = true };
            var sut = new ConditionalExecutionBehavior().AttachTo(operation);

            sut.Execute();

            Assert.False(operation.HasExecuted);
        }

        [Fact]
        public void The_innermost_operation_is_considered_for_conditional_execution()
        {
            var operation = new SkippableOperation { SkipExecution = true };
            var innerBehavior = new FakeOperationBehavior().AttachTo(operation);
            var sut = new ConditionalExecutionBehavior().AttachTo(innerBehavior);

            sut.Execute();

            Assert.False(operation.HasExecuted);
        }

        [Fact]
        public void The_inner_operation_is_not_skipped_when_it_states_that_it_should_not_be()
        {
            var operation = new SkippableOperation { SkipExecution = false };
            var sut = new ConditionalExecutionBehavior().AttachTo(operation);

            sut.Execute();

            Assert.True(operation.HasExecuted);
        }

        [Fact]
        public void The_inner_operation_is_not_skipped_if_it_is_not_a_conditional_operation()
        {
            var operation = new FakeOperation();
            var sut = new ConditionalExecutionBehavior().AttachTo(operation);

            sut.Execute();

            Assert.True(operation.HasExecuted);
        }

        [Theory, AutoMoqData]
        public void Skipped_operations_are_logged(FakeWorkflowLogger log)
        {
            var operation = new SkippableOperation { SkipExecution = true };
            var sut = new ConditionalExecutionBehavior().AttachTo(operation);
            sut.Initialize(new FakeWorkflowConfiguration { Logger = log });

            sut.Execute();

            Assert.Equal(1, log.AppliedBehaviors.Count);
            Assert.Equal("Operation was skipped", log.AppliedBehaviors[0].Description);
        }

        private class SkippableOperation : Operation, IConditionalOperation
        {
            public bool HasExecuted { get; private set; }

            protected override void OnExecute()
            {
                HasExecuted = true;
            }

            public bool SkipExecution { get; set; }
        }
    }
}

using Overflow.Behaviors;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test.Behaviors
{
    public class ConditionalExecutionBehaviorFactoryTests
    {
        [Fact]
        public void A_conditional_execution_behavior_is_created_for_conditional_operations()
        {
            var operation = new SkippableOperation();
            var sut = new ConditionalExecutionBehaviorFactory();

            var result = sut.CreateBehaviors(operation, new FakeWorkflowConfiguration());

            Assert.Equal(1, result.Count);
            Assert.IsType<ConditionalExecutionBehavior>(result[0]);
            Assert.NotNull(result[0]);
        }

        [Fact]
        public void A_conditional_execution_behavior_is_not_created_for_unconditional_operations()
        {
            var operation = new FakeOperation();
            var sut = new ConditionalExecutionBehaviorFactory();

            var result = sut.CreateBehaviors(operation, new FakeWorkflowConfiguration());

            Assert.Equal(0, result.Count);
        }

        private class SkippableOperation : Operation, IConditionalOperation
        {
            protected override void OnExecute() { }

            public bool SkipExecution { get; private set; }
        }
    }
}

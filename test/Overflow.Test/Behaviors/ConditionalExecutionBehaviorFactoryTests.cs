using Overflow.Behaviors;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;

namespace Overflow.Test.Behaviors
{
    public class ConditionalExecutionBehaviorFactoryTests
    {
        [Theory, AutoMoqData]
        public void A_conditional_execution_behavior_is_created_for_conditional_operations(SkippableOperation operation)
        {
            var sut = new ConditionalExecutionBehaviorFactory();

            var result = sut.CreateBehaviors(operation, new FakeWorkflowConfiguration());

            Assert.Equal(1, result.Count);
            Assert.IsType<ConditionalExecutionBehavior>(result[0]);
            Assert.NotNull(result[0]);
        }

        [Theory, AutoMoqData]
        public void A_conditional_execution_behavior_is_not_created_for_unconditional_operations(FakeOperation operation)
        {
            var sut = new ConditionalExecutionBehaviorFactory();

            var result = sut.CreateBehaviors(operation, new FakeWorkflowConfiguration());

            Assert.Equal(0, result.Count);
        }

        public class SkippableOperation : Operation, IConditionalOperation
        {
            public bool SkipExecution { get; private set; }
        }
    }
}

using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class WorkflowTests
    {
        [Fact]
        public void Configuring_a_workflow_returns_a_configuration_instance()
        {
            var result = Workflow.Configure<FakeOperation>();

            Assert.NotNull(result);
        }

        [Fact]
        public void Configurations_are_created_with_default_values()
        {
            var result = Workflow.Configure<FakeOperation>();
            
            Assert.Equal(2, result.BehaviorFactories.Count);
            Assert.IsType<OperationBehaviorAttributeFactory>(result.BehaviorFactories[0]);
            Assert.IsType<OperationLoggingBehaviorFactory>(result.BehaviorFactories[1]);
        }
    }
}

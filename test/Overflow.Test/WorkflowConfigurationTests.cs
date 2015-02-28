using Xunit;

namespace Overflow.Test
{
    public class WorkflowConfigurationTests
    {
        [Fact]
        public void You_can_create_an_operation_from_the_configuration()
        {
            var sut = new WorkflowConfiguration<TestOperation>();

            var result = sut.CreateOperation();

            Assert.NotNull(result);
        }

        private class TestOperation : Operation
        {
            protected override void OnExecute() { }
        }
    }
}

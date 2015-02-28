using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class WorkflowConfigurationTests
    {
        [Fact]
        public void You_can_create_an_operation_from_the_configuration()
        {
            var sut = new WorkflowConfiguration<TestOperation>() { Resolver = new SimpleOperationResolver() };

            var result = sut.CreateOperation();

            Assert.NotNull(result);
        }

        [Fact]
        public void You_can_fluently_assign_the_operation_resolver()
        {
            var sut = new WorkflowConfiguration();
            var resolver = new SimpleOperationResolver();

            sut.WithResolver(resolver);

            Assert.Equal(resolver, sut.Resolver);
        }

        [Fact]
        public void Fluently_assigning_operation_resolver_returns_configuration()
        {
            var sut = new WorkflowConfiguration();

            var result = sut.WithResolver(new SimpleOperationResolver());

            Assert.Equal(sut, result);
        }

        [Fact]
        public void You_can_fluently_assign_the_workflow_logger()
        {
            var sut = new WorkflowConfiguration();
            var logger = new FakeWorkflowLogger();

            sut.WithLogger(logger);

            Assert.Equal(logger, sut.Logger);
        }

        [Fact]
        public void Fluently_assigning_workflow_logger_returns_configuration()
        {
            var sut = new WorkflowConfiguration();

            var result = sut.WithLogger(new FakeWorkflowLogger());

            Assert.Equal(sut, result);
        }

        private class TestOperation : Operation
        {
            protected override void OnExecute() { }
        }
    }
}

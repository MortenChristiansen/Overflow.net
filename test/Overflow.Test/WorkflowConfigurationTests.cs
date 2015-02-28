using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class WorkflowConfigurationTests
    {
        [Fact]
        public void There_are_no_behavior_builders_by_default()
        {
            var sut = new WorkflowConfiguration();

            Assert.NotNull(sut.BehaviorBuilders);
            Assert.Equal(0, sut.BehaviorBuilders.Count);
        }

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

        [Fact]
        public void You_can_fluently_assign_behavior_builders()
        {
            var sut = new WorkflowConfiguration();
            var builder = new FakeOperationBehaviorBuilder();

            sut.WithBehaviorBuilder(builder);

            Assert.Equal(1, sut.BehaviorBuilders.Count);
            Assert.Equal(builder, sut.BehaviorBuilders[0]);
        }

        [Fact]
        public void Fluently_assigning_behavior_builders_returns_configuration()
        {
            var sut = new WorkflowConfiguration();

            var result = sut.WithBehaviorBuilder(new FakeOperationBehaviorBuilder());

            Assert.Equal(sut, result);
        }

        private class TestOperation : Operation
        {
            protected override void OnExecute() { }
        }
    }
}

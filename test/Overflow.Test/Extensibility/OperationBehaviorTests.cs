using System;
using Overflow.Extensibility;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;
using Xunit.Extensions;

namespace Overflow.Test.Extensibility
{
    public class OperationBehaviorTests : TestBase
    {
        private const string Description = "Description";

        [Theory, AutoMoqData]
        public void Attaching_a_behavior_sets_the_inner_operation_property(IOperation operation)
        {
            var sut = new TestBehavior();

            sut.AttachTo(operation);

            Assert.Equal(operation, sut.InnerOperation);
        }

        [Fact]
        public void Guards_are_verified()
        {
            VerifyGuards<TestBehavior>();
        }

        [Theory, AutoMoqData]
        public void Attaching_a_behavior_returns_the_same_behavior(IOperation operation)
        {
            var sut = new TestBehavior();

            var result = sut.AttachTo(operation);

            Assert.Equal(sut, result);
        }

        [Fact]
        public void The_operation_to_decorate_is_required_for_attaching_a_behavior()
        {
            Assert.Throws<ArgumentNullException>(() => new TestBehavior().AttachTo(null));
        }

        [Theory, AutoMoqData]
        public void The_decorated_operation_provides_the_child_operations(IOperation op1, IOperation op2)
        {
            var operation = new FakeOperation(op1, op2);
            var sut = new TestBehavior().AttachTo(operation);

            var result = sut.GetChildOperations();

            Assert.Equal(operation.GetChildOperations(), result);
        }

        [Fact]
        public void The_decorator_forwards_the_execution_to_the_decorated_operation()
        {
            var operation = new FakeOperation();
            var sut = new TestBehavior().AttachTo(operation);

            sut.Execute();

            Assert.True(operation.HasExecuted);
        }

        [Theory, AutoMoqData]
        public void The_decorator_forwards_the_initialization_to_the_decorated_operation(FakeOperation operation)
        {
            var sut = new TestBehavior().AttachTo(operation);
            var configuration = new FakeWorkflowConfiguration();

            sut.Initialize(configuration);

            Assert.Equal(configuration, operation.InitializedConfiguration);
        }

        [Theory, AutoMoqData]
        public void The_decorator_forwards_the_executed_child_operations_to_the_decorated_operation(IOperation operation)
        {
            var sut = new TestBehavior().AttachTo(operation);

            sut.Execute();

            Assert.Equal(operation.ExecutedChildOperations, sut.ExecutedChildOperations);
        }

        [Theory, AutoMoqData]
        public void When_initialized_with_a_logger_the_behavior_log_behavior_applications(IOperation operation, FakeWorkflowLogger log)
        {
            var sut = new TestBehavior();
            sut.AttachTo(operation);
            sut.Initialize(new FakeWorkflowConfiguration { Logger = log });

            sut.ApplyBahviorLog(Description);

            Assert.Equal(1, log.AppliedBehaviors.Count);
            Assert.Equal(sut, log.AppliedBehaviors[0].Behavior);
            Assert.Equal(operation, log.AppliedBehaviors[0].Operation);
            Assert.Equal(Description, log.AppliedBehaviors[0].Description);
        }

        [Theory, AutoMoqData]
        public void Applied_behaviors_are_logged_with_the_innermost_operation(IOperation operation, FakeWorkflowLogger log)
        {
            var behavior = new FakeOperationBehavior().AttachTo(operation);
            var sut = new TestBehavior();
            sut.AttachTo(behavior);
            sut.Initialize(new FakeWorkflowConfiguration { Logger = log });

            sut.ApplyBahviorLog(Description);

            Assert.Equal(operation, log.AppliedBehaviors[0].Operation);
        }

        [Theory, AutoMoqData]
        public void When_initialized_without_a_logger_the_behavior_does_not_logs_behavior_applications(IOperation operation, WorkflowConfiguration configuration)
        {
            var sut = new TestBehavior();
            sut.AttachTo(operation);
            sut.Initialize(configuration);

            sut.ApplyBahviorLog(Description);
        }

        [Theory, AutoMoqData]
        public void When_not_initialized_the_behavior_does_not_log_behavior_applications(IOperation operation)
        {
            var sut = new TestBehavior();
            sut.AttachTo(operation);

            sut.ApplyBahviorLog(Description);
        }

        private class TestBehavior : OperationBehavior
        {
            public override BehaviorPrecedence Precedence
            {
                get { return BehaviorPrecedence.Logging; }
            }

            public void ApplyBahviorLog(string description)
            {
                BehaviorWasApplied(description);
            }
        }
    }
}

using System;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class OperationBehaviorTests
    {
        [Fact]
        public void Attaching_a_behavior_sets_the_inner_operation_property()
        {
            var operation = new FakeOperation();
            var sut = new TestBehavior();

            sut.Attach(operation);

            Assert.Equal(operation, sut.InnerOperation);
        }

        [Fact]
        public void Attaching_a_behavior_returns_the_same_behavior()
        {
            var operation = new FakeOperation();
            var sut = new TestBehavior();

            var result = sut.Attach(operation);

            Assert.Equal(sut, result);
        }

        [Fact]
        public void The_operation_to_decorate_is_required_for_attaching_a_behavior()
        {
            Assert.Throws<ArgumentNullException>(() => new TestBehavior().Attach(null));
        }

        [Fact]
        public void The_decorated_operation_provides_the_child_operations()
        {
            var operation = new FakeOperation(new FakeOperation(), new FakeOperation());
            var sut = new TestBehavior().Attach(operation);

            var result = sut.GetChildOperations();

            Assert.Equal(operation.GetChildOperations(), result);
        }

        [Fact]
        public void The_decorator_forwards_the_execution_to_the_decorated_operation()
        {
            var operation = new FakeOperation();
            var sut = new TestBehavior().Attach(operation);

            sut.Execute();

            Assert.True(operation.HasExecuted);
        }

        [Fact]
        public void The_decorator_forwards_the_initialization_to_the_decorated_operation()
        {
            var operation = new FakeOperation();
            var sut = new TestBehavior().Attach(operation);
            var configuration = new FakeWorkflowConfiguration();

            sut.Initialize(configuration);

            Assert.Equal(configuration, operation.InitializedConfiguration);
        }

        [Fact]
        public void The_decorator_forwards_the_executed_child_operations_to_the_decorated_operation()
        {
            var operation = new FakeOperation();
            var sut = new TestBehavior().Attach(operation);

            sut.Execute();

            Assert.Equal(operation.ExecutedChildOperations, sut.ExecutedChildOperations);
        }

        private class TestBehavior : OperationBehavior
        {
            public override BehaviorIntegrityMode IntegrityMode
            {
                get { return BehaviorIntegrityMode.FullIntegrity; }
            }
        }
    }
}

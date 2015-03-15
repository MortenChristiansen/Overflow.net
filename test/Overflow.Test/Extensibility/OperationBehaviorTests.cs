using System;
using Overflow.Extensibility;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test.Extensibility
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

        [Fact]
        public void When_initialized_with_a_logger_the_behavior_log_behavior_applications()
        {
            var operation = new FakeOperation();
            var sut = new TestBehavior();
            var log = new FakeWorkflowLogger();
            sut.Attach(operation);
            sut.Initialize(new FakeWorkflowConfiguration { Logger = log });

            sut.ApplyBahviorLog("DESCRIPTION");

            Assert.Equal(1, log.AppliedBehaviors.Count);
            Assert.Equal(sut, log.AppliedBehaviors[0].Behavior);
            Assert.Equal(operation, log.AppliedBehaviors[0].Operation);
            Assert.Equal("DESCRIPTION", log.AppliedBehaviors[0].Description);
        }

        [Fact]
        public void Applied_behaviors_are_logged_with_the_innermost_operation()
        {
            var operation = new FakeOperation();
            var behavior = new FakeOperationBehavior().Attach(operation);
            var sut = new TestBehavior();
            var log = new FakeWorkflowLogger();
            sut.Attach(behavior);
            sut.Initialize(new FakeWorkflowConfiguration { Logger = log });

            sut.ApplyBahviorLog("DESCRIPTION");

            Assert.Equal(operation, log.AppliedBehaviors[0].Operation);
        }

        [Fact]
        public void When_initialized_without_a_logger_the_behavior_does_not_logs_behavior_applications()
        {
            var operation = new FakeOperation();
            var sut = new TestBehavior();
            sut.Attach(operation);
            sut.Initialize(new FakeWorkflowConfiguration());

            sut.ApplyBahviorLog("DESCRIPTION");
        }

        [Fact]
        public void When_not_initialized_the_behavior_does_not_log_behavior_applications()
        {
            var operation = new FakeOperation();
            var sut = new TestBehavior();
            sut.Attach(operation);

            sut.ApplyBahviorLog("DESCRIPTION");
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

using System;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class OperationDecoratorTests
    {
        [Fact]
        public void Creating_a_decorator_sets_the_decorated_operation_property()
        {
            var operation = new FakeOperation();

            var sut = new TestDecorator(operation);

            Assert.Equal(operation, sut.DecoratedOperation);
        }

        [Fact]
        public void The_decorated_operation_is_required_for_creating_a_new_decorator()
        {
            Assert.Throws<ArgumentNullException>(() => new TestDecorator(null));
        }

        [Fact]
        public void The_decorated_operation_provides_the_child_operations()
        {
            var operation = new FakeOperation(new FakeOperation(), new FakeOperation());
            var sut = new TestDecorator(operation);

            var result = sut.GetChildOperations();

            Assert.Equal(operation.GetChildOperations(), result);
        }

        [Fact]
        public void The_decorator_fowards_the_execution_to_the_decorated_operation()
        {
            var operation = new FakeOperation();
            var sut = new TestDecorator(operation);

            sut.Execute();

            Assert.True(operation.HasExecuted);
        }

        private class TestDecorator : OperationDecorator
        {
            public TestDecorator(IOperation operation)
                : base(operation)
            {
                
            } 
        }
    }
}

using System;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class OperationBehaviorAttributeTests
    {
        [Fact]
        public void Creating_an_instance_sets_the_operation_type_property()
        {
            var sut = new FakeOperationBehaviorAttribute(typeof(object));

            Assert.Equal(typeof(object), sut.OperationType);
        }

        [Fact]
        public void You_cannot_create_an_instance_without_an_operation_type()
        {
            Assert.Throws<ArgumentNullException>(() => new FakeOperationBehaviorAttribute(null));
        }
    }
}

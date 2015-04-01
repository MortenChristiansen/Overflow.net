using System;
using Overflow.Behaviors;
using Overflow.Test.TestingInfrastructure;
using Xunit;
using Xunit.Extensions;

namespace Overflow.Test
{
    public class CompensatingOperationAttributeTests
    {
        [Theory, AutoMoqData]
        public void You_can_create_a_behavior(WorkflowConfiguration configuration, SimpleOperationResolver resolver)
        {
            var sut = new CompensatingOperationAttribute(typeof(TestOperation));
            configuration.WithResolver(resolver);

            var result = sut.CreateBehavior(configuration);

            Assert.NotNull(result);
            Assert.IsType<CompensatingOperationBehavior>(result);
        }

        [Theory, AutoMoqData]
        public void You_can_create_a_behavior_with_compensated_exception_types(WorkflowConfiguration configuration, SimpleOperationResolver resolver)
        {
            var sut = new CompensatingOperationAttribute(typeof(TestOperation), typeof(Exception));
            configuration.WithResolver(resolver);

            var result = sut.CreateBehavior(configuration);

            Assert.NotNull(result);
            Assert.IsType<CompensatingOperationBehavior>(result);
        }

        [Fact]
        public void You_cannot_create_the_attribute_without_an_operation_type()
        {
            Assert.Throws<ArgumentNullException>(() => new CompensatingOperationAttribute(null));
        }

        [Fact]
        public void You_cannot_create_the_attribute_with_non_operation_types()
        {
            Assert.Throws<ArgumentException>(() => new CompensatingOperationAttribute(typeof(object)));
        }

        private class TestOperation : Operation { }
    }
}

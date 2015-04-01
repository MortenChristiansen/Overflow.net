using System;
using Overflow.Behaviors;
using Xunit;

namespace Overflow.Test
{
    public class RetryAttributeTests
    {
        [Fact]
        public void You_can_create_a_behavior()
        {
            var sut = new RetryAttribute();

            var result = sut.CreateBehavior(null);

            Assert.NotNull(result);
            Assert.IsType<RetryBehavior>(result);
        }

        [Fact]
        public void The_attribute_arguments_are_passed_on_to_behavior()
        {
            var retryErrorTypes = new[] { typeof (Exception) };
            var sut = new RetryAttribute(5, 500, retryErrorTypes);

            var result = (RetryBehavior)sut.CreateBehavior(null);

            Assert.Equal(5, result.TimesToRetry);
            Assert.Equal(TimeSpan.FromMilliseconds(500), result.RetryDelay);
            Assert.Equal(retryErrorTypes, result.RetryExceptionTypes);
        }
    }
}

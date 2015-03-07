using System;
using Xunit;

namespace Overflow.Test
{
    public class RetryAttributeTests
    {
        [Fact]
        public void You_can_create_a_behavior()
        {
            var sut = new RetryAttribute();

            var result = sut.CreateBehavior();

            Assert.NotNull(result);
            Assert.IsType<RetryBehavior>(result);
        }

        [Fact]
        public void The_attribute_arguments_Are_passed_on_to_behavior()
        {
            var sut = new RetryAttribute(5, 500);

            var result = (RetryBehavior)sut.CreateBehavior();

            Assert.Equal(5, result.TimesToRetry);
            Assert.Equal(TimeSpan.FromMilliseconds(500), result.RetryDelay);
        }
    }
}

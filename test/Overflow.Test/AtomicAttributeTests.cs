using Overflow.Behaviors;
using Xunit;

namespace Overflow.Test
{
    public class AtomicAttributeTests
    {
        [Fact]
        public void You_can_create_a_behavior()
        {
            var sut = new AtomicAttribute();

            var result = sut.CreateBehavior();

            Assert.NotNull(result);
            Assert.IsType<AtomicBehavior>(result);
        }
    }
}

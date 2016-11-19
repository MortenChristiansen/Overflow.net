#if NET_FULL
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

            var result = sut.CreateBehavior(null);

            Assert.NotNull(result);
            Assert.IsType<AtomicBehavior>(result);
        }
    }
}
#endif
using Overflow.Behaviors;
using Xunit;

namespace Overflow.Test
{
    public class ContinueOnFailureAttributeTests
    {
        [Fact]
        public void You_can_create_a_behavior()
        {
            var sut = new ContinueOnFailureAttribute();

            var result = sut.CreateBehavior(null);

            Assert.NotNull(result);
            Assert.IsType<ContinueOnFailureBehavior>(result);
        }
    }
}

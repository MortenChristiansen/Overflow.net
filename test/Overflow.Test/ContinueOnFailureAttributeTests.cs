using Xunit;

namespace Overflow.Test
{
    public class ContinueOnFailureAttributeTests
    {
        [Fact]
        public void You_can_create_a_behavior()
        {
            var sut = new ContinueOnFailureAttribute();

            var result = sut.CreateBehavior();

            Assert.NotNull(result);
            Assert.IsType<ContinueOnFailureBehavior>(result);
        }
    }
}

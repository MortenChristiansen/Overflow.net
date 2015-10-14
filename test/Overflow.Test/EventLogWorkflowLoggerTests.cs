using Overflow.Test.TestingInfrastructure;
using Xunit;

namespace Overflow.Test
{
    public class EventLogWorkflowLoggerTests : TestBase
    {
        [Fact]
        public void Guards_are_verified()
        {
            VerifyGuards<EventLogWorkflowLogger>();
        }
    }
}

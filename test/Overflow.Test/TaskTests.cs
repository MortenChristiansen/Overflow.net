using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class TaskTests
    {
        [Fact]
        public void Executing_a_task_calls_the_OnExecute_method()
        {
            var sut = new FakeTask();

            sut.Execute();
            
            Assert.True(sut.HasExecuted);
        }
    }
}

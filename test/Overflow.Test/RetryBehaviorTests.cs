using System;
using Overflow.Test.Fakes;
using Overflow.Utilities;
using Xunit;

namespace Overflow.Test
{
    public class RetryBehaviorTests
    {
        public RetryBehaviorTests()
        {
            Time.Stop();
        }

        [Fact]
        public void The_behavior_maintains_operation_integrity()
        {
            var sut = new RetryBehavior(1, TimeSpan.Zero);

            Assert.Equal(BehaviorIntegrityMode.MaintainsOperationIntegrity, sut.IntegrityMode);
        }

        [Fact]
        public void Failing_operations_are_retried()
        {
            var operation = new FakeOperation { ThrowOnExecute = new Exception(), ErrorCount = 1 };
            var sut = new RetryBehavior(1, TimeSpan.Zero);
            sut.Attach(operation);

            sut.Execute();
        }

        [Fact]
        public void Failing_operations_are_retried_aspoecific_number_of_times()
        {
            var operation = new FakeOperation { ThrowOnExecute = new Exception(), ErrorCount = 2 };
            var sut = new RetryBehavior(1, TimeSpan.Zero);
            sut.Attach(operation);

            Assert.Throws<Exception>(() => sut.Execute());
        }

        [Fact]
        public void You_can_only_retry_a_positive_number_of_times()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RetryBehavior(0, TimeSpan.Zero));
        }

        [Fact]
        public void You_cannot_delay_a_negative_duration()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RetryBehavior(1, TimeSpan.FromMilliseconds(-10)));
        }

        [Fact]
        public void Retries_are_delayed_the_specified_duration()
        {
            var operation = new FakeOperation { ThrowOnExecute = new Exception(), ErrorCount = 1 };
            var sut = new RetryBehavior(1, TimeSpan.FromSeconds(5));
            sut.Attach(operation);
            var before = Time.OffsetUtcNow;

            sut.Execute();

            var duration = Time.OffsetUtcNow - before;
            Assert.Equal(TimeSpan.FromSeconds(5), duration); 
        }
    }
}

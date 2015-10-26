using System;
using System.Diagnostics;
using System.Threading;
using Overflow.Utilities;
using Xunit;

namespace Overflow.Test.Utilities
{
    public class TimeTests
    {
        public TimeTests()
        {
            Time.Start();
        }

        [Fact]
        public void Getting_time_utc_offset_time_returns_the_actual_time()
        {
            var before = DateTimeOffset.UtcNow;
            var now = Time.OffsetUtcNow;
            var after = DateTimeOffset.UtcNow;

            Assert.True(before <= now);
            Assert.True(now <= after);
        }

        [Fact]
        public void You_can_restart_the_time_when_it_has_been_stopped()
        {
            Time.Stop(DateTime.Today);

            var before = DateTimeOffset.UtcNow;
            Time.Start();
            var now = Time.OffsetUtcNow;
            var after = DateTimeOffset.UtcNow;

            Assert.True(before <= now);
            Assert.True(now <= after);
        }

        [Fact]
        public void When_time_is_stopped_getting_time_utc_offset_time_returns_the_time_where_it_was_stopped()
        {
            var stoppedAt = DateTimeOffset.UtcNow.AddHours(1);

            Time.Stop(stoppedAt);
            var time = Time.OffsetUtcNow;

            Assert.Equal(stoppedAt, time);
        }

        [Fact]
        public void Stopping_time_with_no_argument_stops_it_now()
        {
            var before = DateTimeOffset.UtcNow;
            Time.Stop();
            var now = Time.OffsetUtcNow;
            var after = DateTimeOffset.UtcNow;

            Assert.True(before <= now);
            Assert.True(now <= after);
            Assert.Equal(now, Time.OffsetUtcNow);
        }

        [Fact]
        public void Waiting_while_time_is_stopped_updates_the_time_appropriately()
        {
            Time.Stop();
            var before = Time.OffsetUtcNow;

            Time.Wait(TimeSpan.FromMinutes(5));

            var duration = Time.OffsetUtcNow - before;
            Assert.Equal(TimeSpan.FromMinutes(5), duration);
        }

        [Fact]
        public void Measuring_time_returns_the_duration_in_milliseconds()
        {
            var measurement = Time.Measure();
            Wait30Milliseconds();
            var elapsed = measurement.GetElapsedMilliseconds();

            Assert.True(elapsed >= 30, elapsed + "ms");
        }

        private static void Wait30Milliseconds()
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 30)
                Thread.Sleep(1);
        }

        [Fact]
        public void Measuring_stopped_time_returns_the_duration_in_milliseconds()
        {
            Time.Stop();
            var measurement = Time.Measure();
            Time.Stop(Time.OffsetUtcNow.AddMilliseconds(20));
            var elapsed = measurement.GetElapsedMilliseconds();

            Assert.Equal(20, elapsed);
        }
    }
}

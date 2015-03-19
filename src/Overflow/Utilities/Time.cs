using System;
using System.Diagnostics;
using System.Threading;

namespace Overflow.Utilities
{
    static class Time
    {
        private static DateTimeOffset? _stoppedAt;

        public static DateTimeOffset OffsetUtcNow { get { return _stoppedAt ?? DateTimeOffset.UtcNow; } }

        public static void Stop(DateTimeOffset stoppedAt)
        {
            _stoppedAt = stoppedAt;
        }

        public static void Stop()
        {
            _stoppedAt = DateTimeOffset.UtcNow;
        }

        public static void Start()
        {
            _stoppedAt = null;
        }

        public static void Wait(TimeSpan timeSpan)
        {
            if (!_stoppedAt.HasValue)
                new AutoResetEvent(false).WaitOne(timeSpan);
            else
                _stoppedAt += timeSpan;
        }

        public static Measurement Measure()
        {
            return new Measurement();
        }

        public class Measurement
        {
            private readonly Stopwatch _stopwatch;
            private readonly DateTimeOffset? _startedAt;

            internal Measurement()
            {
                if (_stoppedAt.HasValue)
                    _startedAt = OffsetUtcNow;

                _stopwatch = Stopwatch.StartNew();
            }

            public long GetElapsedMilliseconds()
            {
                var elapsed = _stopwatch.ElapsedMilliseconds;

                if (_startedAt.HasValue)
                    return (long)(OffsetUtcNow - _startedAt.Value).TotalMilliseconds;

                return elapsed;
            }
        }
    }
}

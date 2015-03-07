using System;
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
    }
}

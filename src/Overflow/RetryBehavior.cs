using System;
using Overflow.Utilities;

namespace Overflow
{
    class RetryBehavior : OperationBehavior
    {
        private int _timesToRetry;
        private readonly TimeSpan _retryDelay;

        public RetryBehavior(int timesToRetry, TimeSpan retryDelay)
        {
            if (timesToRetry <= 0)
                throw new ArgumentOutOfRangeException("timesToRetry", "Must be larger than 0.");

            if (retryDelay.Ticks < 0)
                throw new ArgumentOutOfRangeException("retryDelay", "Delay must be non-negative.");

            _timesToRetry = timesToRetry;
            _retryDelay = retryDelay;
        }

        public override BehaviorIntegrityMode IntegrityMode
        {
            get { return BehaviorIntegrityMode.MaintainsOperationIntegrity; }
        }

        public override void Execute()
        {
            if (_timesToRetry-- > 0)
            {
                try { base.Execute(); }
                catch
                {
                    Time.Wait(_retryDelay);
                    Execute();
                }
            }
            else
            {
                base.Execute();
            }
        }
    }
}

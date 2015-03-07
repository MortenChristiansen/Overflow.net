using System;
using Overflow.Utilities;

namespace Overflow
{
    class RetryBehavior : OperationBehavior
    {
        public int TimesToRetry { get; private set; }
        public TimeSpan RetryDelay { get; private set; }

        public RetryBehavior(int timesToRetry, TimeSpan retryDelay)
        {
            if (timesToRetry <= 0)
                throw new ArgumentOutOfRangeException("timesToRetry", "Must be larger than 0.");

            if (retryDelay.Ticks < 0)
                throw new ArgumentOutOfRangeException("retryDelay", "Delay must be non-negative.");

            TimesToRetry = timesToRetry;
            RetryDelay = retryDelay;
        }

        public override BehaviorIntegrityMode IntegrityMode
        {
            get { return BehaviorIntegrityMode.MaintainsOperationIntegrity; }
        }

        public override void Execute()
        {
            if (TimesToRetry-- > 0)
            {
                try { base.Execute(); }
                catch
                {
                    Time.Wait(RetryDelay);
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

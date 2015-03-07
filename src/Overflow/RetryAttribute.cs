using System;

namespace Overflow
{
    public class RetryAttribute : OperationBehaviorAttribute
    {
        private readonly int _timesToRetry;
        private readonly int _retryDelayInMilliSeconds;

        public RetryAttribute(int timesToRetry = 3, int retryDelayInMilliSeconds = 1000)
        {
            _timesToRetry = timesToRetry;
            _retryDelayInMilliSeconds = retryDelayInMilliSeconds;
        }

        public override OperationBehavior CreateBehavior()
        {
            return new RetryBehavior(_timesToRetry, TimeSpan.FromMilliseconds(_retryDelayInMilliSeconds));
        }
    }
}

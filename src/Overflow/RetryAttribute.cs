using System;
using Overflow.Behaviors;
using Overflow.Extensibility;

namespace Overflow
{
    public class RetryAttribute : OperationBehaviorAttribute
    {
        private readonly int _timesToRetry;
        private readonly int _retryDelayInMilliSeconds;
        private readonly Type[] _retryExeptionTypes;

        public RetryAttribute(int timesToRetry = 3, int retryDelayInMilliSeconds = 1000, params Type[] retryExeptionTypes)
        {
            _timesToRetry = timesToRetry;
            _retryDelayInMilliSeconds = retryDelayInMilliSeconds;
            _retryExeptionTypes = retryExeptionTypes;
        }

        public override OperationBehavior CreateBehavior()
        {
            return new RetryBehavior(_timesToRetry, TimeSpan.FromMilliseconds(_retryDelayInMilliSeconds), _retryExeptionTypes);
        }
    }
}

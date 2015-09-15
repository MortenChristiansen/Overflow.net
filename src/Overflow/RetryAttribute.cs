using System;
using Overflow.Behaviors;
using Overflow.Extensibility;

namespace Overflow
{
    /// <summary>
    /// Applies the Retry behavior. Failures are retried a number of times. It 
    /// can be limited to only retry specific exceptions. Note that only 
    /// child operations with the Indempotent attribute can be retried.
    /// </summary>
    public class RetryAttribute : OperationBehaviorAttribute
    {
        private readonly int _timesToRetry;
        private readonly int _retryDelayInMilliSeconds;
        private readonly Type[] _retryExeptionTypes;

        /// <summary>
        /// Creates a Retry attribute.
        /// </summary>
        /// <param name="timesToRetry">The number of times to retry before giving up and rethrowing the exception</param>
        /// <param name="retryDelayInMilliSeconds">The delay in milliseconds before each retry.</param>
        /// <param name="retryExeptionTypes">The types of exceptions to retry. If no types are specified, all exceptions
        /// are retried. Exceptions inheriting from the specified types are retried as well.</param>
        public RetryAttribute(int timesToRetry = 3, int retryDelayInMilliSeconds = 1000, params Type[] retryExeptionTypes)
        {
            _timesToRetry = timesToRetry;
            _retryDelayInMilliSeconds = retryDelayInMilliSeconds;
            _retryExeptionTypes = retryExeptionTypes;
        }

        /// <summary>
        /// Create the retry behavior.
        /// </summary>
        /// <param name="configuration">The configuration of the executing workflow</param>
        /// <returns>The created beahvior</returns>
        public override OperationBehavior CreateBehavior(WorkflowConfiguration configuration)
        {
            return new RetryBehavior(_timesToRetry, TimeSpan.FromMilliseconds(_retryDelayInMilliSeconds), _retryExeptionTypes);
        }
    }
}

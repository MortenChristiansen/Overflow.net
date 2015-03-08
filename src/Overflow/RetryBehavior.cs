using System;
using System.Collections.Generic;
using System.Linq;
using Overflow.Utilities;

namespace Overflow
{
    class RetryBehavior : OperationBehavior
    {
        public IEnumerable<Type> RetryExceptionTypes { get; private set; }
        public int TimesToRetry { get; private set; }
        public TimeSpan RetryDelay { get; private set; }

        public RetryBehavior(int timesToRetry, TimeSpan retryDelay, params Type[] retryExceptionTypes)
        {
            if (timesToRetry <= 0)
                throw new ArgumentOutOfRangeException("timesToRetry", "Must be larger than 0.");

            if (retryDelay.Ticks < 0)
                throw new ArgumentOutOfRangeException("retryDelay", "Delay must be non-negative.");

            if(retryExceptionTypes.Any(t => !typeof(Exception).IsAssignableFrom(t)))
                throw new ArgumentException("Only exception types are valid.", "retryExceptionTypes");

            RetryExceptionTypes = retryExceptionTypes;
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
                catch (Exception e)
                {
                    if (HasExecutedNonIndempotentChildOperations() || !IsConfiguredToRetryType(e.GetType()))
                        throw;

                    Time.Wait(RetryDelay);
                    Execute();
                }
            }
            else
            {
                base.Execute();
            }
        }

        private bool IsConfiguredToRetryType(Type exceptionType)
        {
            return RetryExceptionTypes == null || !RetryExceptionTypes.Any() || RetryExceptionTypes.Any(t => t.IsAssignableFrom(exceptionType));
        }

        private bool HasExecutedNonIndempotentChildOperations()
        {
            return InnerOperation.GetExecutedChildOperationsForOperationHierarchy().Any(o => !IsIndempotent(o.Operation));
        }

        private bool IsIndempotent(IOperation operation)
        {
            return operation.GetType().GetCustomAttributes(typeof(IndempotentAttribute), false).Any();
        }
    }
}

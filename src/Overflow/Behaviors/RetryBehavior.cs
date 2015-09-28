using System;
using System.Collections.Generic;
using System.Linq;
using Overflow.Extensibility;
using Overflow.Utilities;

namespace Overflow.Behaviors
{
    class RetryBehavior : OperationBehavior
    {
        public IEnumerable<Type> RetryExceptionTypes { get; }
        public int TimesToRetry { get; private set; }
        public TimeSpan RetryDelay { get; }

        public RetryBehavior(int timesToRetry, TimeSpan retryDelay, params Type[] retryExceptionTypes)
        {
            Verify.LargerThanZero(timesToRetry, "Must be larger than 0.");
            Verify.LargeThanOrEqualToZero(retryDelay.Ticks, "Delay must be non-negative.");
            Verify.Argument(retryExceptionTypes.All(t => typeof (Exception).IsAssignableFrom(t)), "Only exception types are valid.");

            RetryExceptionTypes = retryExceptionTypes;
            TimesToRetry = timesToRetry;
            RetryDelay = retryDelay;
        }

        public override BehaviorPrecedence Precedence => BehaviorPrecedence.WorkRecovery;

        public override void Execute()
        {
            if (TimesToRetry-- > 0)
            {
                try { base.Execute(); }
                catch (Exception e) when (!HasExecutedNonIdempotentChildOperations() && IsConfiguredToRetryType(e.GetType()))
                {
                    Time.Wait(RetryDelay);
                    BehaviorWasApplied("Operation retried");
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

        private bool HasExecutedNonIdempotentChildOperations()
        {
            return InnerOperation.GetExecutedChildOperationsForOperationHierarchy().Any(o => !IsIdempotent(o.Operation));
        }

        private static bool IsIdempotent(IOperation operation)
        {
            return operation.GetType().GetCustomAttributes(typeof(IdempotentAttribute), false).Any();
        }
    }
}

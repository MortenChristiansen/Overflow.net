using System;
using System.Collections.Generic;
using System.Linq;
using Overflow.Behaviors;
using Overflow.Utilities;

namespace Overflow.Testing
{
    /// <summary>
    /// Extension methods for the IOperation interface.
    /// </summary>
    public static class OperationExtensions
    {
        /// <summary>
        /// Assert that the operation has executed child operations of the specified
        /// types, in the specified order. The entire hierarchy of child operations is
        /// included.
        /// </summary>
        /// <param name="operation">The operation to verify child operations for</param>
        /// <param name="expectedOperationTypes">The types of child operations expected</param>
        public static void ExecutesChildOperations(this IOperation operation, params Type[] expectedOperationTypes)
        {
            new ContinueOnFailureBehavior().AttachTo(operation).Execute();
            VerifyExecutedOperations(operation, expectedOperationTypes, allowOperationFailures: true);
        }

        private static void VerifyExecutedOperations(IOperation operation, Type[] expectedOperationTypes, bool allowOperationFailures)
        {
            Verify.NotNull(operation, nameof(operation));
            Verify.NotNull(expectedOperationTypes, nameof(expectedOperationTypes));

            var messageParts = new List<string>(expectedOperationTypes.Length + 2) { "Operations", "==========" };
            var executedOperations = operation.GetExecutedChildOperationsForOperationHierarchy().ToList();
            var items = Math.Max(expectedOperationTypes.Length, executedOperations.Count);
            var hasErrors = false;
            for (var i = 0; i < items; i++)
            {
                if (i >= executedOperations.Count)
                {
                    messageParts.Add("none [error: expected " + expectedOperationTypes[i].Name + "]");
                    hasErrors = true;
                    continue;
                }

                var executedOperationType = executedOperations[i].Operation.GetType();

                if (i >= expectedOperationTypes.Length)
                {
                    messageParts.Add(executedOperationType.Name + " [error: expected none]");
                    hasErrors = true;
                    continue;
                }

                if (executedOperationType != expectedOperationTypes[i])
                {
                    messageParts.Add(executedOperationType.Name + " [error: expected " + expectedOperationTypes[i].Name + "]");
                    hasErrors = true;
                    continue;
                }

                var hasFailedOperation = !allowOperationFailures && executedOperations[i].Error != null;
                hasErrors = hasErrors || hasFailedOperation;
                var matchMessage = hasFailedOperation ? " [match, failed]" : " [match]";
                messageParts.Add(expectedOperationTypes[i].Name + matchMessage);
            }

            if (hasErrors)
                throw new AssertionException(string.Join(Environment.NewLine, messageParts));
        }

        /// <summary>
        /// Assert that the operation has executed child operations of the specified
        /// types, in the specified order. The entire hierarchy of child operations is
        /// included. It is verified that none of the executed operations has thrown an
        /// exception.
        /// </summary>
        /// <param name="operation">The operation to verify child operations for</param>
        /// <param name="expectedOperationTypes">The types of child operations expected</param>
        public static void ExecutesChildOperationsWithoutErrors(this IOperation operation, params Type[] expectedOperationTypes)
        {
            new ContinueOnFailureBehavior().AttachTo(operation).Execute();
            VerifyExecutedOperations(operation, expectedOperationTypes, allowOperationFailures: false);
        }
    }
}

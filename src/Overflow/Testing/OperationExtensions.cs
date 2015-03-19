using System;
using System.Collections.Generic;
using System.Linq;
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
        public static void HasExecutedChildOperations(this IOperation operation, params Type[] expectedOperationTypes)
        {
            Verify.NotNull(operation, "operation");
            Verify.NotNull(expectedOperationTypes, "expectedOperationTypes");

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

                messageParts.Add(expectedOperationTypes[i].Name + " [match]");
            }

            if (hasErrors)
                throw new AssertionException(string.Join(Environment.NewLine, messageParts));
        }
    }
}

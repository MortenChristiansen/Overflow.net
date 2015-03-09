using System;
using System.Collections.Generic;
using System.Linq;

namespace Overflow.Testing
{
    public static class OperationExtensions
    {
        public static void HasExecutedChildOperations(this IOperation operation, params Type[] expectedOperationTypes)
        {
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

using System.Collections.Generic;
using Overflow.Extensibility;
using Overflow.Utilities;

namespace Overflow
{
    /// <summary>
    /// One step in a larger workflow. Operations can be isolated or
    /// contain child operations.
    /// </summary>
    public interface IOperation
    {
        /// <summary>
        /// Get the child operations which have been executed during the execution of this
        /// operation.
        /// </summary>
        IEnumerable<ExecutionInfo> ExecutedChildOperations { get; }
 
        /// <summary>
        /// Initialize the operation. 
        /// </summary>
        /// <param name="configuration">The global configuration of the workflow</param>
        void Initialize(WorkflowConfiguration configuration);
        /// <summary>
        /// Run the logic of the operation.
        /// </summary>
        void Execute();
        /// <summary>
        /// Yield the child operations of the operation. This collection should always be
        /// evaluated one at a time, executing each task before retreiving the next. Otherwise,
        /// correct execution cannot be expected.
        /// </summary>
        /// <returns>Any child operations. Note that the execution of each child operation can
        /// influence the next sibling operations.</returns>
        IEnumerable<IOperation> GetChildOperations();
    }

    public static class IOperationExtensions
    {
        /// <summary>
        /// Retreive all the executed child operations in the entire hierarchy of child operations
        /// underneath the current operation.
        /// </summary>
        /// <param name="parentOperation">The root operation</param>
        /// <returns>All executed child operations</returns>
        public static IList<ExecutionInfo> GetExecutedChildOperationsForOperationHierarchy(this IOperation parentOperation)
        {
            Verify.NotNull(parentOperation, "parentOperation");

            var result = new List<ExecutionInfo>();

            foreach (var execution in parentOperation.ExecutedChildOperations)
            {
                result.Add(execution);
                result.AddRange(execution.Operation.GetExecutedChildOperationsForOperationHierarchy());
            }

            return result;
        }

        /// <summary>
        /// Retrieve the original operation without beahviors.
        /// </summary>
        /// <param name="operation">An operation, possibly having behaviors attached to it</param>
        /// <returns>The original operation</returns>
        public static IOperation GetInnermostOperation(this IOperation operation)
        {
            Verify.NotNull(operation, "operation");

            while (true)
            {
                var behavior = operation as OperationBehavior;
                if (behavior != null)
                {
                    operation = behavior.InnerOperation;
                    continue;
                }

                return operation;
            }
        }
    }
}

using System.Collections.Generic;
using Overflow.Extensibility;

namespace Overflow
{
    public interface IOperation
    {
        IEnumerable<ExecutionInfo> ExecutedChildOperations { get; }
 
        void Initialize(WorkflowConfiguration configuration);
        void Execute();
        IEnumerable<IOperation> GetChildOperations();
    }

    public static class IOperationExtensions
    {
        public static IList<ExecutionInfo> GetExecutedChildOperationsForOperationHierarchy(this IOperation parentOperation)
        {
            var result = new List<ExecutionInfo>();

            foreach (var execution in parentOperation.ExecutedChildOperations)
            {
                result.Add(execution);
                result.AddRange(execution.Operation.GetExecutedChildOperationsForOperationHierarchy());
            }

            return result;
        }

        public static IOperation GetInnermostOperation(this IOperation operation)
        {
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

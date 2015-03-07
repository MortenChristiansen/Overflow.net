using System.Collections.Generic;

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
    }
}

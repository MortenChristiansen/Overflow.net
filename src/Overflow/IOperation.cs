using System.Collections.Generic;

namespace Overflow
{
    public interface IOperation
    {
        void Initialize(WorkflowConfiguration configuration);
        void Execute();
        IEnumerable<IOperation> GetChildOperations();
    }
}

using System.Collections.Generic;

namespace Overflow
{
    public interface IOperation
    {
        void Execute();
        IEnumerable<IOperation> GetChildOperations();
    }
}

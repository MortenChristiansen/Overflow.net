using System;

namespace Overflow
{
    public interface IWorkflowLogger
    {
        void OperationStarted(IOperation operation);
        void OperationFinished(IOperation operation);
        void OperationFailed(IOperation operation, Exception error);
    }
}

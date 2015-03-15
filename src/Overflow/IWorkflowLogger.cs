using System;
using Overflow.Extensibility;

namespace Overflow
{
    public interface IWorkflowLogger
    {
        void OperationStarted(IOperation operation);
        void OperationFinished(IOperation operation);
        void OperationFailed(IOperation operation, Exception error);
        void BehaviorWasApplied(IOperation operation, OperationBehavior behavior, string description);
    }
}

using System;
using Overflow.Extensibility;

namespace Overflow
{
    /// <summary>
    /// Logs the execution flow of a workflow. 
    /// </summary>
    public interface IWorkflowLogger
    {
        /// <summary>
        /// Log that an operation has started executing.
        /// </summary>
        /// <param name="operation">The executing operation</param>
        void OperationStarted(IOperation operation);
        /// <summary>
        /// Log an operation has finished executing.
        /// </summary>
        /// <param name="operation">The executing operation</param>
        void OperationFinished(IOperation operation);
        /// <summary>
        /// Log an exception while executing an operation.
        /// </summary>
        /// <param name="operation">The executing operation</param>
        /// <param name="error">The exception being thrown</param>
        void OperationFailed(IOperation operation, Exception error);
        /// <summary>
        /// Log that a behavior was applied, modifying the normal
        /// execution flow.
        /// </summary>
        /// <param name="operation">The executing operation</param>
        /// <param name="behavior">The beahvior being applied</param>
        /// <param name="description">A description of how the behavior modified the exectuion
        /// flow</param>
        void BehaviorWasApplied(IOperation operation, OperationBehavior behavior, string description);
    }
}

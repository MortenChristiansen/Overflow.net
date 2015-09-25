using System;
using Overflow.Utilities;

namespace Overflow
{
    /// <summary>
    /// Information about the execution of an operation.
    /// </summary>
    public class ExecutionInfo
    {
        /// <summary>
        /// Gets the executed operation.
        /// </summary>
        public IOperation Operation { get; }
        /// <summary>
        /// Gets any Exception which might have been thrown during the execution.
        /// Returns null if no exceptions where thrown.
        /// </summary>
        public Exception Error { get; }
        /// <summary>
        /// The time at which the execution started.
        /// </summary>
        public DateTimeOffset Started { get; }
        /// <summary>
        /// The time at which the execution completed.
        /// </summary>
        public DateTimeOffset Completed { get; }

        /// <summary>
        /// Create new execution info.
        /// </summary>
        /// <param name="operation">The executed operation</param>
        /// <param name="error">Any error which occurred during the execution or null</param>
        /// <param name="started">When the operation execution was started</param>
        /// <param name="completed">When the operation execution was completed</param>
        public ExecutionInfo(IOperation operation, Exception error, DateTimeOffset started, DateTimeOffset completed)
        {
            Verify.NotNull(operation, nameof(operation));

            Operation = operation;
            Error = error;
            Started = started;
            Completed = completed;
        }
    }
}

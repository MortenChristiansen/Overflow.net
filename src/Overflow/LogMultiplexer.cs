using System;
using System.Collections.Generic;
using System.Linq;
using Overflow.Extensibility;
using Overflow.Utilities;

namespace Overflow
{
    /// <summary>
    /// A logger that forwards logs to any number of actual loggers. Use this if
    /// you need to log to more than one destination.
    /// </summary>
    public class LogMultiplexer : IWorkflowLogger
    {
        private readonly List<IWorkflowLogger> _loggers;

        /// <summary>
        /// Creates a new instance of the log multiplexer.
        /// </summary>
        /// <param name="loggers">Logger implementations to invoke
        /// whenever a log entry is added.</param>
        public LogMultiplexer(params IWorkflowLogger[] loggers)
        {
            Verify.NotNull(loggers, nameof(loggers));

            _loggers = loggers.ToList();
        }

        /// <summary>
        /// Call OperationStarted on each supplied logger.
        /// </summary>
        /// <param name="operation">The operation is passed on the each logger</param>
        public void OperationStarted(IOperation operation)
        {
            Verify.NotNull(operation, nameof(operation));

            _loggers.ForEach(l => l.OperationStarted(operation));
        }

        /// <summary>
        /// Call OperationFinished on each supplied logger.
        /// </summary>
        /// <param name="operation">The operation is passed on the each logger</param>
        /// <param name="duration">The duration is passed on the each logger</param>
        public void OperationFinished(IOperation operation, TimeSpan duration)
        {
            Verify.NotNull(operation, nameof(operation));

            _loggers.ForEach(l => l.OperationFinished(operation, duration));
        }

        /// <summary>
        /// Call OperationFailed on each supplied logger.
        /// </summary>
        /// <param name="operation">The operation is passed on the each logger</param>
        /// <param name="error">The error is passed on the each logger</param>
        public void OperationFailed(IOperation operation, Exception error)
        {
            Verify.NotNull(operation, nameof(operation));
            Verify.NotNull(error, nameof(error));

            _loggers.ForEach(l => l.OperationFailed(operation, error));
        }

        /// <summary>
        /// Call BehaviorWasApplied on each supplied logger.
        /// </summary>
        /// <param name="operation">The operation is passed on the each logger</param>
        /// <param name="behavior">The behavior is passed on the each logger</param>
        /// <param name="description">The description is passed on the each logger</param>
        public void BehaviorWasApplied(IOperation operation, OperationBehavior behavior, string description)
        {
            Verify.NotNull(operation, nameof(operation));
            Verify.NotNull(behavior, nameof(behavior));
            Verify.NotNull(description, nameof(description));

            _loggers.ForEach(l => l.BehaviorWasApplied(operation, behavior, description));
        }
    }
}

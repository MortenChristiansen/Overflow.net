using System;
using System.Diagnostics;
using System.Globalization;
using Overflow.Extensibility;
using Overflow.Utilities;

namespace Overflow
{
    /// <summary>
    /// A logger than writes to the event log. Note that this logger requires the
    /// application to execute with special permissions. You can either run as 
    /// administrator or configure the machine to allow writing to the log. See 
    /// https://support.microsoft.com/en-us/kb/2028427 for details.
    /// </summary>
    public class EventLogWorkflowLogger : IWorkflowLogger
    {
        private readonly string _source;
        private readonly string _logName;

        /// <summary>
        /// Create a new logger.
        /// </summary>
        /// <param name="source">The source that will appear in the event logs. It should identify the 
        /// application.</param>
        /// <param name="logName">The log in which the entries will be written. Can be Application,
        /// Security or a custom event log</param>
        public EventLogWorkflowLogger(string source, string logName = "Application")
        {
            Verify.NotNull(source, nameof(source));
            Verify.NotNull(logName, nameof(logName));

            _source = source;
            _logName = logName;
        }

        /// <summary>
        /// Create en event log entry stating that an operation has started.
        /// </summary>
        /// <param name="operation">The operation that started</param>
        public void OperationStarted(IOperation operation)
        {
            Verify.NotNull(operation, nameof(operation));

            LogMessage($"{operation.GetType().Name} started");
        }

        /// <summary>
        /// Create en event log entry stating that an operation has finished,
        /// including its duration.
        /// </summary>
        /// <param name="operation">The operation that finished</param>
        /// <param name="duration"></param>
        public void OperationFinished(IOperation operation, TimeSpan duration)
        {
            Verify.NotNull(operation, nameof(operation));

            var durationFormatted = string.Format(CultureInfo.CurrentCulture, "{0:#,###,###,##0}", duration.TotalMilliseconds);
            LogMessage($"{operation.GetType().Name} completed in {durationFormatted}ms");
        }

        /// <summary>
        /// Create en event log entry stating that an operation failed,
        /// including a description of the error.
        /// </summary>
        /// <param name="operation">The operation that failed</param>
        /// <param name="error">The error that occurred</param>
        public void OperationFailed(IOperation operation, Exception error)
        {
            Verify.NotNull(operation, nameof(operation));
            Verify.NotNull(error, nameof(error));

            var log =
$@"{operation.GetType().Name} failed

{error.GetType().Name}: {error.Message}
{error.StackTrace}";

            LogMessage(log, EventLogEntryType.Warning);
        }

        /// <summary>
        /// Create en event log entry stating that a behavior was applied to an operation.
        /// </summary>
        /// <param name="operation">The operation for which the behavior was applied</param>
        /// <param name="behavior">The behavior that was applied</param>
        /// <param name="description">A description of the behavior that was applied</param>
        public void BehaviorWasApplied(IOperation operation, OperationBehavior behavior, string description)
        {
            Verify.NotNull(operation, nameof(operation));
            Verify.NotNull(behavior, nameof(behavior));
            Verify.NotNull(description, nameof(description));

            LogMessage($"{behavior.GetType().Name} was applied to {operation.GetType().Name}: {description}");
        }

        private void LogMessage(string message, EventLogEntryType type = EventLogEntryType.Information)
        {
            if (!EventLog.SourceExists(_source))
                EventLog.CreateEventSource(_source, _logName);

            EventLog.WriteEntry(_source, message, type);
        }
    }
}

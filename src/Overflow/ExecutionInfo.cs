using System;

namespace Overflow
{
    public class ExecutionInfo
    {
        public IOperation Operation { get; private set; }
        public Exception Error { get; private set; }
        public DateTimeOffset Started { get; private set; }
        public DateTimeOffset Completed { get; private set; }

        public ExecutionInfo(IOperation operation, Exception error, DateTimeOffset started, DateTimeOffset completed)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            Operation = operation;
            Error = error;
            Started = started;
            Completed = completed;
        }
    }
}

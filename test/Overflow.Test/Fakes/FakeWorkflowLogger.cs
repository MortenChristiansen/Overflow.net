using System;
using System.Collections.Generic;

namespace Overflow.Test.Fakes
{
    class FakeWorkflowLogger : IWorkflowLogger
    {
        public IList<IOperation> StartedOperations { get; private set; }
        public IList<IOperation> FinishedOperations { get; private set; }
        public IDictionary<IOperation, IList<Exception>> OperationFailures { get; private set; }

        public FakeWorkflowLogger()
        {
            StartedOperations = new List<IOperation>();
            FinishedOperations = new List<IOperation>();
            OperationFailures = new Dictionary<IOperation, IList<Exception>>();
        }

        public void OperationStarted(IOperation operation)
        {
            StartedOperations.Add(operation);
        }

        public void OperationFinished(IOperation operation)
        {
            FinishedOperations.Add(operation);
        }

        public void OperationFailed(IOperation operation, Exception error)
        {
            if(!OperationFailures.ContainsKey(operation))
                OperationFailures.Add(operation, new List<Exception>());

            OperationFailures[operation].Add(error);
        }
    }
}

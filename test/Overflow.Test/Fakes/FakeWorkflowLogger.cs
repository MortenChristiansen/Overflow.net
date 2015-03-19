using System;
using System.Collections.Generic;
using Overflow.Extensibility;

namespace Overflow.Test.Fakes
{
    public class FakeWorkflowLogger : IWorkflowLogger
    {
        public IList<IOperation> StartedOperations { get; private set; }
        public IList<IOperation> FinishedOperations { get; private set; }
        public IList<TimeSpan> FinishedOperationDurations { get; private set; }
        public IDictionary<IOperation, IList<Exception>> OperationFailures { get; private set; }
        public IList<BehaviorApplication> AppliedBehaviors { get; private set; }

        public FakeWorkflowLogger()
        {
            StartedOperations = new List<IOperation>();
            FinishedOperations = new List<IOperation>();
            FinishedOperationDurations = new List<TimeSpan>();
            OperationFailures = new Dictionary<IOperation, IList<Exception>>();
            AppliedBehaviors = new List<BehaviorApplication>();
        }

        public void OperationStarted(IOperation operation)
        {
            StartedOperations.Add(operation);
        }

        public void OperationFinished(IOperation operation, TimeSpan duration)
        {
            FinishedOperations.Add(operation);
            FinishedOperationDurations.Add(duration);
        }

        public void OperationFailed(IOperation operation, Exception error)
        {
            if(!OperationFailures.ContainsKey(operation))
                OperationFailures.Add(operation, new List<Exception>());

            OperationFailures[operation].Add(error);
        }

        public void BehaviorWasApplied(IOperation operation, OperationBehavior behavior, string description)
        {
            AppliedBehaviors.Add(new BehaviorApplication{ Behavior = behavior, Operation = operation, Description = description });
        }

        public class BehaviorApplication
        {
            public IOperation Operation { get; set; }
            public OperationBehavior Behavior { get; set; }
            public string Description { get; set; }
        }
    }
}

using System;
using System.Collections.Generic;

namespace Overflow.Extensibility
{
    public abstract class OperationBehavior : IOperation
    {
        private IWorkflowLogger _logger;
        internal IOperation InnerOperation { get; private set; }

        public abstract BehaviorPrecedence Precedence { get; }

        public IOperation Attach(IOperation innerOperation)
        {
            if (innerOperation == null)
                throw new ArgumentNullException("innerOperation");

            InnerOperation = innerOperation;

            return this;
        }

        public IEnumerable<ExecutionInfo> ExecutedChildOperations { get { return InnerOperation.ExecutedChildOperations; } }

        public void Initialize(WorkflowConfiguration configuration)
        {
            _logger = configuration.Logger;

            InnerOperation.Initialize(configuration);
        }

        public virtual void Execute()
        {
            InnerOperation.Execute();
        }

        public virtual IEnumerable<IOperation> GetChildOperations()
        {
            return InnerOperation.GetChildOperations();
        }

        protected void BehaviorWasApplied(string description)
        {
            if (_logger != null)
                _logger.BehaviorWasApplied(InnerOperation.GetInnermostOperation(), this, description);
        }
    }
}

using System;
using Overflow.Extensibility;

namespace Overflow.Behaviors
{
    class OperationExecutionLoggingBehavior : OperationBehavior
    {
        private readonly IWorkflowLogger _logger;

        public override BehaviorPrecedence Precedence
        {
            get { return BehaviorPrecedence.Logging; }
        }

        public OperationExecutionLoggingBehavior(IWorkflowLogger logger)
        {
            _logger = logger;
        }

        public override void Execute()
        {
            var innermostOperation = this.GetInnermostOperation();

            _logger.OperationStarted(innermostOperation);
            try { base.Execute(); }
            finally
            {
                _logger.OperationFinished(innermostOperation);
            }
        }
    }
}

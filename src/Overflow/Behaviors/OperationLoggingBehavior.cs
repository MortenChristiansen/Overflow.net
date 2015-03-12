using System;
using Overflow.Extensibility;

namespace Overflow.Behaviors
{
    class OperationLoggingBehavior : OperationBehavior
    {
        private readonly IWorkflowLogger _logger;

        public override BehaviorPrecedence Precedence
        {
            get { return BehaviorPrecedence.Logging; }
        }

        public OperationLoggingBehavior(IWorkflowLogger logger)
        {
            _logger = logger;
        }

        public override void Execute()
        {
            var innermostOperation = GetInnerOperation(InnerOperation);

            _logger.OperationStarted(innermostOperation);
            try { base.Execute(); }
            catch (Exception e)
            {
                _logger.OperationFailed(innermostOperation, e);
                throw;
            }
            finally
            {
                _logger.OperationFinished(innermostOperation);
            }
        }

        private IOperation GetInnerOperation(IOperation operation)
        {
            var behavior = operation as OperationBehavior;
            if (behavior != null)
                return GetInnerOperation(behavior.InnerOperation);

            return operation;
        }
    }
}

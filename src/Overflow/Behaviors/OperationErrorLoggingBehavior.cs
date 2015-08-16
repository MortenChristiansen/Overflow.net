using System;
using Overflow.Extensibility;

namespace Overflow.Behaviors
{
    class OperationErrorLoggingBehavior : OperationBehavior
    {
        private readonly IWorkflowLogger _logger;

        public override BehaviorPrecedence Precedence => BehaviorPrecedence.PreRecovery;

        public OperationErrorLoggingBehavior(IWorkflowLogger logger)
        {
            _logger = logger;
        }

        public override void Execute()
        {
            try { base.Execute(); }
            catch (Exception e)
            when (Log(e)) { }
        }

        private bool Log(Exception e)
        {
            _logger.OperationFailed(InnerOperation.GetInnermostOperation(), e);
            return false;
        }
    }
}

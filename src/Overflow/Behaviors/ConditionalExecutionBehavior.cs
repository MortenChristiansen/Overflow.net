using Overflow.Extensibility;

namespace Overflow.Behaviors
{
    class ConditionalExecutionBehavior : OperationBehavior
    {
        public override BehaviorPrecedence Precedence
        {
            get { return BehaviorPrecedence.Staging; }
        }

        public override void Execute()
        {
            var conditionalOperation = this.GetInnermostOperation() as IConditionalOperation;
            if (conditionalOperation == null || !conditionalOperation.SkipExecution)
                base.Execute();
        }
    }
}

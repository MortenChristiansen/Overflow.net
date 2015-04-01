using Overflow.Extensibility;
using Overflow.Utilities;

namespace Overflow.Behaviors
{
    class CompensatingOperationBehavior : OperationBehavior
    {
        private readonly IOperation _operation;

        public override BehaviorPrecedence Precedence
        {
            get { return BehaviorPrecedence.WorkCompensation; }
        }

        public CompensatingOperationBehavior(IOperation operation)
        {
            Verify.NotNull(operation, "operation");

            _operation = operation;
        }

        public override void Execute()
        {
            try { base.Execute(); }
            catch
            {
                var context = OperationContext.Create(InnerOperation.GetInnermostOperation());
                context.ProvideInputs(_operation);
                _operation.Execute();
                throw;
            }
        }
    }
}

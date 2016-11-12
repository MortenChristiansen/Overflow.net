using System;
using System.Linq;
using Overflow.Extensibility;
using Overflow.Utilities;
using System.Reflection;

namespace Overflow.Behaviors
{
    class CompensatingOperationBehavior : OperationBehavior
    {
        private readonly IOperation _operation;
        private readonly Type[] _compensatedExceptionTypes;

        public override BehaviorPrecedence Precedence => BehaviorPrecedence.WorkCompensation;

        public CompensatingOperationBehavior(IOperation operation, params Type[] compensatedExceptionTypes)
        {
            Verify.NotNull(operation, nameof(operation));
            Verify.Argument(compensatedExceptionTypes.All(t => typeof(Exception).IsAssignableFrom(t)), "Only exception types are valid.");

            _operation = operation;
            _compensatedExceptionTypes = compensatedExceptionTypes;
        }

        public override void Execute()
        {
            try { base.Execute(); }
            catch (Exception e) 
            when (ExecuteCompensatingOperation(e)) {}
        }

        private bool ExecuteCompensatingOperation(Exception e)
        {
            if (_compensatedExceptionTypes.Length == 0 || _compensatedExceptionTypes.Any(t => t.IsInstanceOfType(e)))
                ExecuteCompensatingOperation();

            return false;
        }

        private void ExecuteCompensatingOperation()
        {
            BehaviorWasApplied("Executing compensating operation");
            var context = OperationContext.Create(InnerOperation.GetInnermostOperation());
            context.ProvideInputs(_operation);
            _operation.Execute();
        }
    }
}

using System;
using System.Reflection;
using Overflow.Behaviors;
using Overflow.Extensibility;
using Overflow.Utilities;

namespace Overflow
{
    public class CompensatingOperationAttribute : OperationBehaviorAttribute
    {
        private readonly Type _operationType;

        public CompensatingOperationAttribute(Type operationType)
        {
            Verify.NotNull(operationType, "operationType");
            Verify.Argument(typeof(IOperation).IsAssignableFrom(operationType), "The operation type must implement the IOperation interface");

            _operationType = operationType;
        }

        public override OperationBehavior CreateBehavior(WorkflowConfiguration configuration)
        {
            return new CompensatingOperationBehavior(CreateCompensatingOperation(configuration));
        }

        private Operation CreateCompensatingOperation(WorkflowConfiguration configuration)
        {
            var createMethod = typeof (Operation).GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
            var methodWithTypeArgument = createMethod.MakeGenericMethod(_operationType);
            return (Operation)methodWithTypeArgument.Invoke(null, new object[] { configuration });
        }
    }
}

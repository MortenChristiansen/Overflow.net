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
        private readonly Type[] _compensatedExceptionTypes;

        public CompensatingOperationAttribute(Type operationType, params Type[] compensatedExceptionTypes)
        {
            Verify.NotNull(operationType, "operationType");
            Verify.Argument(typeof(IOperation).IsAssignableFrom(operationType), "The operation type must implement the IOperation interface");

            _operationType = operationType;
            _compensatedExceptionTypes = compensatedExceptionTypes;
        }

        public override OperationBehavior CreateBehavior(WorkflowConfiguration configuration)
        {
            return new CompensatingOperationBehavior(CreateCompensatingOperation(configuration), _compensatedExceptionTypes);
        }

        private Operation CreateCompensatingOperation(WorkflowConfiguration configuration)
        {
            var createMethod = typeof (Operation).GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
            var methodWithTypeArgument = createMethod.MakeGenericMethod(_operationType);
            return (Operation)methodWithTypeArgument.Invoke(null, new object[] { configuration });
        }
    }
}

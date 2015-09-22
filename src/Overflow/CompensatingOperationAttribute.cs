using System;
using System.Reflection;
using Overflow.Behaviors;
using Overflow.Extensibility;
using Overflow.Utilities;

namespace Overflow
{
    /// <summary>
    /// Applies the Compensating Operation beahvior. When errors occur during the execution
    /// of the operation, an instance of the specified compensating operation type is created and executed.
    /// This operation gets behaviors applied as normal and can implement the IInputOperation
    /// interface. The IOutputOperation interface will be ignored.
    /// 
    /// Note that the exception thrown in the original exception is rethrown once the 
    /// compensating operation is done executing. Consider using the ContinueOnFailure attribute
    /// if this is not the desired behavior.
    /// 
    /// If the compensating operation throws an exception, the original exception is not rethrown,
    /// and the new exception bubbles up instead. To avoid this, make the compensating operation
    /// use the ContinueOnFailure attribute.
    /// </summary>
    public class CompensatingOperationAttribute : OperationBehaviorAttribute
    {
        private readonly Type _operationType;
        private readonly Type[] _compensatedExceptionTypes;

        /// <summary>
        /// Creates a new instance of the attribute.
        /// </summary>
        /// <param name="operationType">The type of compensating operation to create in
        /// case of exceptions. The type must implement the IOperation interface.</param>
        /// <param name="compensatedExceptionTypes">Optionally supply specific exception types
        /// to limit the compensating operations to. By default, all exceptions are compensated
        /// for. Exceptions inheriting from one of the specified types will be compensated for 
        /// as well. The types must inherit from Exception.</param>
        public CompensatingOperationAttribute(Type operationType, params Type[] compensatedExceptionTypes)
        {
            Verify.NotNull(operationType, nameof(operationType));
            Verify.Argument(typeof(IOperation).IsAssignableFrom(operationType), "The operation type must implement the IOperation interface");

            _operationType = operationType;
            _compensatedExceptionTypes = compensatedExceptionTypes;
        }

        /// <summary>
        /// Creates a new OperationBehavior instance.
        /// </summary>
        /// <param name="configuration">The configuration of the executing workflow</param>
        public override OperationBehavior CreateBehavior(WorkflowConfiguration configuration)
        {
            return new CompensatingOperationBehavior(CreateCompensatingOperation(configuration), _compensatedExceptionTypes);
        }

        private IOperation CreateCompensatingOperation(WorkflowConfiguration configuration)
        {
            var createMethod = typeof (Operation).GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
            var methodWithTypeArgument = createMethod.MakeGenericMethod(_operationType);
            return (IOperation)methodWithTypeArgument.Invoke(null, new object[] { configuration });
        }
    }
}

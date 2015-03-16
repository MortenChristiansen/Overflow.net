using System.Collections.Generic;

namespace Overflow.Extensibility
{
    /// <summary>
    /// The type used for creating behaviors. Factory implementations are
    /// registered using the WorkflowConfiguration class - either directly
    /// or through the Workflow class.
    /// </summary>
    public interface IOperationBehaviorFactory
    {
        /// <summary>
        /// Creates the appropriate behaviors for the operation, given the configuration.
        /// </summary>
        /// <param name="operation">The operation to create behaviors for. Typically, the
        /// operation is inspected using mechanisms such as reflection or type sniffing,
        /// to determine which behaviors the type subscribes to.</param>
        /// <param name="configuration">The workflow configuration. Provides global settings
        /// for the workflow.</param>
        /// <returns>Zero or more behaviors which should be attached to the operation</returns>
        IList<OperationBehavior> CreateBehaviors(IOperation operation, WorkflowConfiguration configuration);
    }
}

using Overflow.Behaviors;
using Overflow.Extensibility;

namespace Overflow
{
    /// <summary>
    /// Applies the Continue On Failure behavior. Failures executing the operation are ignored.
    /// </summary>
    public class ContinueOnFailureAttribute : OperationBehaviorAttribute
    {
        /// <summary>
        /// Creates a new OperationBehavior instance.
        /// </summary>
        /// <param name="configuration">The configuration of the executing workflow</param>
        public override OperationBehavior CreateBehavior(WorkflowConfiguration configuration)
        {
            return new ContinueOnFailureBehavior();
        }
    }
}

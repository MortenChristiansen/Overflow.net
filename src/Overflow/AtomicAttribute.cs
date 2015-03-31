using Overflow.Behaviors;
using Overflow.Extensibility;

namespace Overflow
{
    /// <summary>
    /// Applies the Atomic behavior. The operation is wrapped in a
    /// transaction which is committed if the operation does not fail.
    /// </summary>
    public class AtomicAttribute : OperationBehaviorAttribute
    {
        /// <summary>
        /// Create a new atomic behavior instance.
        /// </summary>
        /// <param name="configuration">The configuration of the executing workflow</param>
        /// <returns>The behavior</returns>
        public override OperationBehavior CreateBehavior(WorkflowConfiguration configuration)
        {
            return new AtomicBehavior();
        }
    }
}

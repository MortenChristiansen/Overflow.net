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
        public override OperationBehavior CreateBehavior()
        {
            return new ContinueOnFailureBehavior();
        }
    }
}

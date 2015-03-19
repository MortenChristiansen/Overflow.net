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
        /// <returns>The behavior</returns>
        public override OperationBehavior CreateBehavior()
        {
            return new AtomicBehavior();
        }
    }
}

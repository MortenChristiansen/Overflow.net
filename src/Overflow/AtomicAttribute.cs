using Overflow.Behaviors;
using Overflow.Extensibility;

namespace Overflow
{
    public class AtomicAttribute : OperationBehaviorAttribute
    {
        public override OperationBehavior CreateBehavior()
        {
            return new AtomicBehavior();
        }
    }
}

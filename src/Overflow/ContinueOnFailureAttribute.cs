using Overflow.Behaviors;
using Overflow.Extensibility;

namespace Overflow
{
    public class ContinueOnFailureAttribute : OperationBehaviorAttribute
    {
        public override OperationBehavior CreateBehavior()
        {
            return new ContinueOnFailureBehavior();
        }
    }
}

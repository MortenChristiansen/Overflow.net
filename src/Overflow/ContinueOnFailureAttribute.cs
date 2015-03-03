namespace Overflow
{
    public class ContinueOnFailureAttribute : OperationBehaviorAttribute
    {
        public override OperationBehavior CreateBehavior()
        {
            return null; //new ContinueOnFailureOperationBehavior(operation);
        }
    }
}

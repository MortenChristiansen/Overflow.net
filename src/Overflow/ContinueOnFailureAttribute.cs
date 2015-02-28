namespace Overflow
{
    public class ContinueOnFailureAttribute : OperationBehaviorAttribute
    {
        public override IOperation AddBehavior(IOperation operation)
        {
            return new ContinueOnFailureOperationBehavior(operation);
        }
    }
}

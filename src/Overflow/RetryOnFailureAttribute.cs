namespace Overflow
{
    public class RetryOnFailureAttribute : OperationBehaviorAttribute
    {
        public override IOperation AddBehavior(IOperation operation)
        {
            return new RetryOnFailureOperationDecorator(operation);
        }
    }
}

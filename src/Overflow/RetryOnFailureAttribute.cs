namespace Overflow
{
    public class RetryOnFailureAttribute : OperationBehaviorAttribute
    {
        public RetryOnFailureAttribute() : base(typeof(RetryOnFailureOperationDecorator)) { }
    }
}

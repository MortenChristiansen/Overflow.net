namespace Overflow
{
    class RetryOnFailureOperationDecorator : OperationDecorator
    {
        public RetryOnFailureOperationDecorator(IOperation decoratedOperation) : base(decoratedOperation) { }

        public override void Execute()
        {
            try { base.Execute(); }
            catch { }
        }
    }
}

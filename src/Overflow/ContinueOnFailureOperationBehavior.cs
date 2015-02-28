namespace Overflow
{
    class ContinueOnFailureOperationBehavior : OperationBehavior
    {
        public ContinueOnFailureOperationBehavior(IOperation innerOperation) : base(innerOperation) { }

        public override void Execute()
        {
            try { base.Execute(); }
            catch { }
        }
    }
}

namespace Overflow
{
    class ContinueOnFailureBehavior : OperationBehavior
    {
        public override BehaviorIntegrityMode IntegrityMode
        {
            get { return BehaviorIntegrityMode.MaintainsOperationIntegrity; }
        }

        public override void Execute()
        {
            try { base.Execute(); }
            catch { }
        }
    }
}

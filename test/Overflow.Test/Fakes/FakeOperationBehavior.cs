namespace Overflow.Test.Fakes
{
    class FakeOperationBehavior : OperationBehavior
    {
        public BehaviorIntegrityMode SetIntegrityMode { get; set; }

        public override BehaviorIntegrityMode IntegrityMode
        {
            get { return SetIntegrityMode; }
        }
    }
}

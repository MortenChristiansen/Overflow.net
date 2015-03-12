using Overflow.Extensibility;

namespace Overflow.Behaviors
{
    class ContinueOnFailureBehavior : OperationBehavior
    {
        public override BehaviorPrecedence Precedence
        {
            get { return BehaviorPrecedence.Containment; }
        }

        public override void Execute()
        {
            try { base.Execute(); }
            catch { }
        }
    }
}

using Overflow.Extensibility;

namespace Overflow.Test.Fakes
{
    class FakeOperationBehavior : OperationBehavior
    {
        public BehaviorPrecedence SetPrecedence { get; set; }

        public override BehaviorPrecedence Precedence
        {
            get { return SetPrecedence; }
        }
    }
}

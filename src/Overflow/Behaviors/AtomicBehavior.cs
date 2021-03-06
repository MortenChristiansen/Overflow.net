using System.Transactions;
using Overflow.Extensibility;

namespace Overflow.Behaviors
{
    class AtomicBehavior : OperationBehavior
    {
        public override BehaviorPrecedence Precedence => BehaviorPrecedence.StateRecovery;

        public override void Execute()
        {
            using (var ts = new TransactionScope())
            {
                base.Execute();

                ts.Complete();
            }
        }
    }
}

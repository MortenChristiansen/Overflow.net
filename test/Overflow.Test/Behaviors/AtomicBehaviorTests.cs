/*
using System;
using System.Transactions;
using Overflow.Behaviors;
using Overflow.Extensibility;
using Overflow.Test.TestingInfrastructure;
using Xunit;

namespace Overflow.Test.Behaviors
{
    public class AtomicBehaviorTests : TestBase
    {
        [Fact]
        public void The_behavior_has_state_recovery_level_precedence()
        {
            var sut = new AtomicBehavior();

            Assert.Equal(BehaviorPrecedence.StateRecovery, sut.Precedence);
        }

        [Fact]
        public void Operations_are_executed_in_a_transaction()
        {
            var operation = new TransactionOperation();
            var sut = new AtomicBehavior().AttachTo(operation);

            sut.Execute();

            Assert.True(operation.ExecutedInTransaction);
        }

        [Fact]
        public void Transactions_are_committed_for_successful_operations()
        {
            var operation = new TransactionOperation();
            var sut = new AtomicBehavior().AttachTo(operation);

            sut.Execute();

            Assert.True(operation.TransactionCommitted);
        }

        [Fact]
        public void Transactions_are_not_committed_for_failed_operations()
        {
            var operation = new TransactionOperation { ThrowOnExecute = new Exception() };
            var sut = new AtomicBehavior().AttachTo(operation);

            ExecuteIgnoringErrors(sut.Execute);

            Assert.False(operation.TransactionCommitted);
        }

        private class TransactionOperation : Operation
        {
            public bool ExecutedInTransaction { get; private set; }
            public bool TransactionCommitted { get; private set; }
            public Exception ThrowOnExecute { get; set; }

            protected override void OnExecute()
            {
                ExecutedInTransaction = Transaction.Current != null;
                if (Transaction.Current != null)
                    Transaction.Current.TransactionCompleted += Current_TransactionCompleted;

                if (ThrowOnExecute != null)
                    throw ThrowOnExecute;
            }

            void Current_TransactionCompleted(object sender, TransactionEventArgs e)
            {
                TransactionCommitted = e.Transaction.TransactionInformation.Status == TransactionStatus.Committed;
                e.Transaction.TransactionCompleted -= Current_TransactionCompleted;
            }
        }
    }
}
*/

using System;
using Overflow.Behaviors;
using Overflow.Extensibility;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Overflow.Utilities;
using Xunit;

namespace Overflow.Test.Behaviors
{
    public class RetryBehaviorTests
    {
        public RetryBehaviorTests()
        {
            Time.Stop();
        }

        [Fact]
        public void The_behavior_has_work_recovery_level_precedence()
        {
            var sut = new RetryBehavior(1, TimeSpan.Zero);

            Assert.Equal(BehaviorPrecedence.WorkRecovery, sut.Precedence);
        }

        [Fact]
        public void Retry_properties_are_populated_from_constructor_arguments()
        {
            var retryErrorTypes = new[] { typeof (Exception) };
            var sut = new RetryBehavior(3, TimeSpan.FromSeconds(5), retryErrorTypes);

            Assert.Equal(3, sut.TimesToRetry);
            Assert.Equal(TimeSpan.FromSeconds(5), sut.RetryDelay);
            Assert.Equal(retryErrorTypes, sut.RetryExceptionTypes);
        }

        [Fact]
        public void You_cannot_add_a_retry_exception_type_that_is_not_an_exception_type()
        {
            Assert.Throws<ArgumentException>(() => new RetryBehavior(1, TimeSpan.Zero, typeof (object)));
        }

        [Fact]
        public void Failing_operations_are_retried()
        {
            var operation = new FakeOperation { ThrowOnExecute = new Exception(), ErrorCount = 1 };
            var sut = new RetryBehavior(1, TimeSpan.Zero);
            sut.AttachTo(operation);

            sut.Execute();
        }

        [Fact]
        public void Failing_operations_are_retried_a_specific_number_of_times()
        {
            var operation = new FakeOperation { ThrowOnExecute = new Exception(), ErrorCount = 2 };
            var sut = new RetryBehavior(1, TimeSpan.Zero);
            sut.AttachTo(operation);

            Assert.Throws<Exception>(() => sut.Execute());
        }

        [Fact]
        public void You_can_only_retry_a_positive_number_of_times()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RetryBehavior(0, TimeSpan.Zero));
        }

        [Fact]
        public void You_cannot_delay_a_negative_duration()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RetryBehavior(1, TimeSpan.FromMilliseconds(-10)));
        }

        [Fact]
        public void Retries_are_delayed_the_specified_duration()
        {
            var operation = new FakeOperation { ThrowOnExecute = new Exception(), ErrorCount = 1 };
            var sut = new RetryBehavior(1, TimeSpan.FromSeconds(5));
            sut.AttachTo(operation);
            var before = Time.OffsetUtcNow;

            sut.Execute();

            var duration = Time.OffsetUtcNow - before;
            Assert.Equal(TimeSpan.FromSeconds(5), duration); 
        }

        [Fact]
        public void You_cannot_retry_an_operation_where_a_non_idempotent_child_operation_has_executed()
        {
            var operation = new FakeOperation(new IdempotentOperation(), new FakeOperation { ThrowOnExecute = new Exception(), ErrorCount = 1 });
            var sut = new RetryBehavior(1, TimeSpan.Zero);
            sut.AttachTo(operation);

            Assert.Throws<Exception>(() => sut.Execute());
        }

        [Fact]
        public void You_can_retry_an_operation_where_an_idempotent_child_operation_has_executed()
        {
            var operation = new FakeOperation(new IdempotentOperation { ThrowOnExecute = new Exception(), ErrorCount = 1 });
            var sut = new RetryBehavior(1, TimeSpan.Zero);
            sut.AttachTo(operation);

            sut.Execute();
        }

        [Fact]
        public void When_retry_exception_types_are_specified_errors_of_these_types_will_be_retried()
        {
            var operation = new FakeOperation { ThrowOnExecute = new NullReferenceException(), ErrorCount = 1 };
            var sut = new RetryBehavior(1, TimeSpan.Zero, typeof(NullReferenceException));
            sut.AttachTo(operation);

            sut.Execute();
        }

        [Fact]
        public void When_retry_exception_types_are_specified_errors_of_sub_types_will_be_retried()
        {
            var operation = new FakeOperation { ThrowOnExecute = new ArgumentNullException(), ErrorCount = 1 };
            var sut = new RetryBehavior(1, TimeSpan.Zero, typeof(ArgumentException));
            sut.AttachTo(operation);

            sut.Execute();
        }

        [Fact]
        public void When_retry_exception_types_are_specified_errors_of_different_types_will_not_be_retried()
        {
            var operation = new FakeOperation { ThrowOnExecute = new NullReferenceException(), ErrorCount = 1 };
            var sut = new RetryBehavior(1, TimeSpan.Zero, typeof(FieldAccessException));
            sut.AttachTo(operation);

            Assert.Throws<NullReferenceException>(() => sut.Execute());
        }

        [Theory, AutoMoqData]
        public void Retried_operations_are_logged(Exception error, FakeWorkflowLogger log)
        {
            var sut = new RetryBehavior(2, TimeSpan.Zero).AttachTo(new FakeOperation { ThrowOnExecute = error, ErrorCount = 2 });
            sut.Initialize(new FakeWorkflowConfiguration { Logger = log });

            sut.Execute();

            Assert.Equal(2, log.AppliedBehaviors.Count);
            Assert.Equal("Operation retried", log.AppliedBehaviors[0].Description);
        }

        [Idempotent]
        private class IdempotentOperation : FakeOperation { }
    }
}

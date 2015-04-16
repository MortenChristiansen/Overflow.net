using System;
using Overflow.Behaviors;
using Overflow.Extensibility;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;

namespace Overflow.Test.Behaviors
{
    public class CompensatingOperationBehaviorTests
    {
        [Fact]
        public void The_behavior_has_work_compensation_level_precedence()
        {
            var sut = new CompensatingOperationBehavior(new FakeOperation());

            Assert.Equal(BehaviorPrecedence.WorkCompensation, sut.Precedence);
        }

        [Fact]
        public void Errors_compensated_for_are_rethrown()
        {
            var operation = new FakeOperation { ThrowOnExecute = new Exception() };
            var sut = new CompensatingOperationBehavior(new FakeOperation());
            sut.AttachTo(operation);

            Assert.Throws<Exception>(() => sut.Execute());
        }

        [Fact]
        public void When_an_error_occurs_in_the_operation_the_compensating_operation_is_immediately_executed()
        {
            var operation = new FakeOperation { ThrowOnExecute = new Exception() };
            var compensatingOperation = new FakeOperation();
            var sut = new CompensatingOperationBehavior(compensatingOperation);
            sut.AttachTo(operation);

            try { sut.Execute(); }
            catch { }

            Assert.True(compensatingOperation.HasExecuted);
        }

        [Fact]
        public void You_cannot_create_a_compensating_operation_behavior_without_an_operation_to_compensate_with()
        {
            Assert.Throws<ArgumentNullException>(() => new CompensatingOperationBehavior(null));
        }

        [Theory, AutoMoqData]
        public void Compensating_operations_have_input_values_supplied_from_the_original_operation(object input)
        {
            var operation = new FakeInputOperation<object> { ThrowOnExecute = new Exception() };
            var compensatingOperation = new FakeInputOperation<object>();
            var sut = new CompensatingOperationBehavior(compensatingOperation);
            operation.Input(input);
            var parentOperation = new FakeOperation(new FakeOutputOperation<object> { OutputValue = input }, sut.AttachTo(new FakeOperationBehavior().AttachTo(operation)));

            try { parentOperation.Execute(); }
            catch { }

            Assert.True(compensatingOperation.InputWasProvided);
            Assert.Equal(input, compensatingOperation.ProvidedInput);
        }

        [Theory, AutoMoqData]
        public void You_can_specify_compensated_error_types_with_exception_types(IOperation operation)
        {
            new CompensatingOperationBehavior(operation, typeof(Exception));
        }

        [Theory, AutoMoqData]
        public void You_cannot_specify_compensated_error_types_with_non_exception_types(IOperation operation)
        {
            Assert.Throws<ArgumentException>(() => new CompensatingOperationBehavior(operation, typeof(object)));
        }

        [Fact]
        public void When_specifying_a_compensated_exception_type_other_types_of_exceptions_will_not_be_compensated()
        {
            var operation = new FakeOperation { ThrowOnExecute = new Exception() };
            var compensatingOperation = new FakeOperation();
            var sut = new CompensatingOperationBehavior(compensatingOperation, typeof(ArgumentException));
            sut.AttachTo(operation);

            try { sut.Execute(); }
            catch { }

            Assert.False(compensatingOperation.HasExecuted);
        }

        [Fact]
        public void When_specifying_a_compensated_exception_type_the_compensating_operation_will_be_executed_in_case_of_such_an_exception()
        {
            var operation = new FakeOperation { ThrowOnExecute = new ArgumentException() };
            var compensatingOperation = new FakeOperation();
            var sut = new CompensatingOperationBehavior(compensatingOperation, typeof(ArgumentException));
            sut.AttachTo(operation);

            try { sut.Execute(); }
            catch { }

            Assert.True(compensatingOperation.HasExecuted);
        }

        [Fact]
        public void When_specifying_a_compensated_exception_type_the_compensating_operation_will_be_executed_in_case_of_an_exception_inheriting_from_this_exception()
        {
            var operation = new FakeOperation { ThrowOnExecute = new ArgumentException() };
            var compensatingOperation = new FakeOperation();
            var sut = new CompensatingOperationBehavior(compensatingOperation, typeof(Exception));
            sut.AttachTo(operation);

            try { sut.Execute(); }
            catch { }

            Assert.True(compensatingOperation.HasExecuted);
        }

        [Theory, AutoMoqData]
        public void Executing_a_compensating_operation_logs_an_event(FakeWorkflowLogger log)
        {
            var operation = new FakeOperation { ThrowOnExecute = new ArgumentException() };
            var compensatingOperation = new FakeOperation();
            var sut = new CompensatingOperationBehavior(compensatingOperation, typeof(Exception));
            sut.AttachTo(operation);
            sut.Initialize(new FakeWorkflowConfiguration { Logger = log });

            try { sut.Execute(); }
            catch { }

            Assert.Equal(1, log.AppliedBehaviors.Count);
            Assert.Equal("Executing compensating operation", log.AppliedBehaviors[0].Description);
        }

        [Theory, AutoMoqData]
        public void When_no_compensating_action_is_triggered_no_event_is_logged(FakeWorkflowLogger log)
        {
            var operation = new FakeOperation { ThrowOnExecute = new Exception() };
            var compensatingOperation = new FakeOperation();
            var sut = new CompensatingOperationBehavior(compensatingOperation, typeof(ArgumentException));
            sut.AttachTo(operation);
            sut.Initialize(new FakeWorkflowConfiguration { Logger = log });

            try { sut.Execute(); }
            catch { }

            Assert.Equal(0, log.AppliedBehaviors.Count);
        }
    }
}

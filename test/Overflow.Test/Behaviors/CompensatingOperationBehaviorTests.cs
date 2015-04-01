using System;
using Overflow.Behaviors;
using Overflow.Extensibility;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;
using Xunit.Extensions;

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
            sut.Attach(operation);

            Assert.Throws<Exception>(() => sut.Execute());
        }

        [Fact]
        public void When_an_error_occurs_in_the_operation_the_compensating_operation_is_immediately_executed()
        {
            var operation = new FakeOperation { ThrowOnExecute = new Exception() };
            var compensatingOperation = new FakeOperation();
            var sut = new CompensatingOperationBehavior(compensatingOperation);
            sut.Attach(operation);

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
            var parentOperation = new FakeOperation(new FakeOutputOperation<object> { OutputValue = input }, sut.Attach( new FakeOperationBehavior().Attach(operation)));

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
            sut.Attach(operation);

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
            sut.Attach(operation);

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
            sut.Attach(operation);

            try { sut.Execute(); }
            catch { }

            Assert.True(compensatingOperation.HasExecuted);
        }

        /* Questions
         * - How to handle multiple applications of the behavior? Disallow? I think we should disallow multiple values to keep it simple
         * 
         * I think there are too many issues running the operation outside of the normal flow. I would have to think about how every future
         * feature works with it. Just adding another operation to the normal execution floor does not break anything. We would of course
         * log the event that the compensating operation is scheduled.
         * 
         * Another approach is to let the compensating operation be a simple Func<bool>. We execute it in the catch block and if it returns
         * false, we rethrow the exception. We would have to add a way to inject any needed parameters. It could be done by adding the
         * configuration as a parameter to the OperationBehaviorAttribute.CreateBehavior method. It annoys me that I won't have the full
         * power of operations. The compensating operation could have any number of child operations, supporting arbitrarily complex 
         * logic, with full logging and other behaviors. Would the func need to be executed in its own try/catch and what would happen
         * on error?
         * 
         * The attribute should take optional parameters for filtering errors by type. So the result is a new abstract attribute type
         * inheriting from OperationBehaviorAttribute which has an abstract method for providing the Func<bool>.
         * 
         * Scheduling operations:
         * 
         * public override void OnExecute()
         * {
         *     Schedule<OperationA>(); // What about input/output data?
         * }
         * 
         * in behavior:
         * 
         * public override void Execute()
         * {
         *     try
         *     {
         *         base.Execute();   
         *     }
         *     catch
         *     {
         *          Schedule<OperationA>(); // It cannot be private to Operation then
         *          
         *          base.Schedule<OperationA>(); // Alternative if the method is public 
         *     }
         * }
         */
    }
}

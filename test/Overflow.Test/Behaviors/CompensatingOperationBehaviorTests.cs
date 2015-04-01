using System;
using Overflow.Behaviors;
using Overflow.Extensibility;
using Overflow.Test.Fakes;
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

        // What about the execution context of the compensating operation? And input data? And what about the
        // impact of retry behaviors?

        /* Questions
         * - Should the CO have input data?
         * - Should the CO provide output data?
         * - Should the CO be allowed the full behavior support?
         * - How to handle failures in the CO? It would not be logged as a failure if the logging behavior is not applied
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

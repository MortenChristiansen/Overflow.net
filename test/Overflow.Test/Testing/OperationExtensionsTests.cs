using System;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;
using Overflow.Testing;

namespace Overflow.Test.Testing
{
    public class OperationExtensionsTests : TestBase
    {
        [Fact]
        public void You_can_verify_that_executing_an_operation_has_executed_a_specific_sequence_of_child_operations()
        {
            var operation = new FakeOperation(new Operation1(), new FakeOperation(new Operation2()));

            operation.ExecutesChildOperations(typeof(Operation1), typeof(FakeOperation), typeof(Operation2));
        }

        [Fact]
        public void You_can_verify_that_executing_an_operation_has_executed_a_specific_sequence_of_child_operations_when_operations_have_behaviors()
        {
            var operation = new FakeOperation(new Operation1(), new FakeOperationBehavior().AttachTo(new FakeOperation(new Operation2())));

            operation.ExecutesChildOperations(typeof(Operation1), typeof(FakeOperation), typeof(Operation2));
        }

        [Fact]
        public void You_get_an_assertion_exception_thrown_when_asserting_that_executing_an_operation_has_executed_a_specific_sequence_of_child_operations_when_it_has_not()
        {
            var operation = new FakeOperation(new FakeOperation(new Operation2()), new Operation1());

            Assert.Throws<AssertionException>(() => operation.ExecutesChildOperations(typeof (Operation1), typeof (FakeOperation), typeof (Operation2)));
        }

        [Fact]
        public void Errors_from_the_execution_do_not_bubble_out_when_asserting_the_execution_order()
        {
            var operation = new FakeOperation{ ThrowOnExecute = new Exception() };

            Assert.Throws<AssertionException>(() => operation.ExecutesChildOperations(typeof (Operation1)));
        }

        [Fact]
        public void You_can_verify_that_executing_an_operation_has_executed_a_specific_sequence_of_child_operations_even_though_an_operation_fails()
        {
            var operation = new FakeOperation(new FakeOperation { ThrowOnExecute = new Exception() });

            operation.ExecutesChildOperations(typeof(FakeOperation));
        }

        [Fact]
        public void Asserting_a_sequence_of_child_operation_executions_highlight_differences_between_expected_and_execute_operations()
        {
            var operation = new FakeOperation(new Operation1(), new FakeOperation(new Operation1()));
            AssertionException exception = null;

            try { operation.ExecutesChildOperations(typeof(Operation1), typeof(FakeOperation), typeof(Operation2)); }
            catch (AssertionException e) { exception = e; }

            var formattedErrorMessage = string.Format("Operations{0}=========={0}Operation1 [match]{0}FakeOperation [match]{0}Operation1 [error: expected Operation2]", NL);
            Assert.Equal(formattedErrorMessage, exception.Message);
        }

        [Fact]
        public void Asserting_a_sequence_of_child_operation_executions_highlight_when_there_are_unexpected_operations_executed()
        {
            var operation = new FakeOperation(new Operation1(), new FakeOperation(new Operation1()));
            AssertionException exception = null;

            try { operation.ExecutesChildOperations(typeof(Operation1), typeof(FakeOperation)); }
            catch (AssertionException e) { exception = e; }

            var formattedErrorMessage = string.Format("Operations{0}=========={0}Operation1 [match]{0}FakeOperation [match]{0}Operation1 [error: expected none]", NL);
            Assert.Equal(formattedErrorMessage, exception.Message);
        }

        [Fact]
        public void Asserting_a_sequence_of_child_operation_executions_highlight_when_there_are_too_few_operations_executed()
        {
            var operation = new FakeOperation(new Operation1(), new FakeOperation());
            AssertionException exception = null;

            try { operation.ExecutesChildOperations(typeof(Operation1), typeof(FakeOperation), typeof(Operation2)); }
            catch (AssertionException e) { exception = e; }

            var formattedErrorMessage = string.Format("Operations{0}=========={0}Operation1 [match]{0}FakeOperation [match]{0}none [error: expected Operation2]", NL);
            Assert.Equal(formattedErrorMessage, exception.Message);
        }

        [Fact]
        public void Asserting_a_sequence_of_child_operation_executions_highlight_matches_after_errors()
        {
            var operation = new FakeOperation(new Operation1(), new FakeOperation(new Operation1()), new Operation1());
            AssertionException exception = null;

            try { operation.ExecutesChildOperations(typeof(Operation1), typeof(FakeOperation), typeof(Operation2), typeof(Operation1)); }
            catch (AssertionException e) { exception = e; }

            var formattedErrorMessage = string.Format("Operations{0}=========={0}Operation1 [match]{0}FakeOperation [match]{0}Operation1 [error: expected Operation2]{0}Operation1 [match]", NL);
            Assert.Equal(formattedErrorMessage, exception.Message);
        }

        [Fact]
        public void You_can_verify_that_executing_an_operation_has_executed_a_specific_sequence_of_child_operations_without_failures()
        {
            var operation = new FakeOperation(new Operation1(), new FakeOperation(new Operation2()));

            operation.ExecutesChildOperationsWithoutErrors(typeof(Operation1), typeof(FakeOperation), typeof(Operation2));
        }

        [Fact]
        public void Asserting_a_sequence_of_child_operation_executions_without_failures_highlight_differences_between_expected_and_execute_operations()
        {
            var operation = new FakeOperation(new Operation1(), new FakeOperation(new Operation1()));
            AssertionException exception = null;

            try { operation.ExecutesChildOperationsWithoutErrors(typeof(Operation1), typeof(FakeOperation), typeof(Operation2)); }
            catch (AssertionException e) { exception = e; }

            var formattedErrorMessage = string.Format("Operations{0}=========={0}Operation1 [match]{0}FakeOperation [match]{0}Operation1 [error: expected Operation2]", NL);
            Assert.Equal(formattedErrorMessage, exception.Message);
        }

        [Fact]
        public void Asserting_a_sequence_of_child_operation_executions_without_failures_throws_an_exception_when_an_operation_has_failed()
        {
            var operation = new FakeOperation(new ContinuingOperation());

            Assert.Throws<AssertionException>(() => operation.ExecutesChildOperationsWithoutErrors(typeof (ContinuingOperation)));
        }

        [Fact]
        public void Asserting_a_sequence_of_child_operation_executions_without_failures_highlight_when_an_operation_failed_but_matched_the_expected_type()
        {
            var operation = new FakeOperation(new ContinuingOperation());
            AssertionException exception = null;

            try { operation.ExecutesChildOperationsWithoutErrors(typeof(ContinuingOperation)); }
            catch (AssertionException e) { exception = e; }

            var formattedErrorMessage = string.Format("Operations{0}=========={0}ContinuingOperation [match, failed]", NL);
            Assert.Equal(formattedErrorMessage, exception.Message);
        }

        private class Operation1 : Operation { }

        private class Operation2 : Operation { }

        [ContinueOnFailure]
        private class ContinuingOperation : Operation
        {
            protected override void OnExecute()
            {
                throw new Exception();
            }
        }
    }
}
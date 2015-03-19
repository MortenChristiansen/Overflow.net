using System;
using System.Collections.Generic;
using System.Linq;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Overflow.Utilities;
using Xunit;
using Xunit.Extensions;

namespace Overflow.Test
{
    public class OperationTests
    {
        [Fact]
        public void Executing_an_operation_calls_the_OnExecute_method()
        {
            var sut = new FakeOperation();

            sut.Execute();
            
            Assert.True(sut.HasExecuted);
        }

        [Fact]
        public void Operations_do_not_have_any_child_operations_by_default()
        {
            var sut = new TestOperation();

            var result = sut.GetChildOperations();

            Assert.False(result.Any());
        }

        [Fact]
        public void Executing_an_operation_executes_each_child_operation_as_well()
        {
            var op1 = new FakeOperation();
            var op2 = new FakeOperation();
            var sut = new FakeOperation(op1, op2);

            sut.Execute();

            Assert.Equal(3, FakeOperation.ExecutedOperations.Count);
            Assert.Equal(sut, FakeOperation.ExecutedOperations[0]);
            Assert.Equal(op1, FakeOperation.ExecutedOperations[1]);
            Assert.Equal(op2, FakeOperation.ExecutedOperations[2]);
        }

        [Theory, AutoMoqData]
        public void You_can_create_operation_with_a_configuration_containing_a_resolver(IOperationResolver resolver)
        {
            var correctConfiguration = new FakeWorkflowConfiguration { Resolver = resolver };

            var result = Operation.Create<TestOperation>(correctConfiguration);

            Assert.NotNull(result);
        }

        [Fact]
        public void Creating_an_operation_initializes_it_with_the_configuration_of_the_parent_operation()
        {
            var correctConfiguration = new FakeWorkflowConfiguration { Resolver = new SimpleOperationResolver() };

            var result = (TestOperation)Operation.Create<TestOperation>(correctConfiguration);

            Assert.Equal(correctConfiguration, result.Configuration);
        }

        [Fact]
        public void You_cannot_create_operations_when_the_workflow_configuration_is_not_set()
        {
            Assert.Throws<InvalidOperationException>(() => Operation.Create<TestOperation>(null));
        }

        [Theory, AutoMoqData]
        public void You_cannot_create_operations_when_the_operation_resolver_is_not_set(WorkflowConfiguration configuration)
        {
            Assert.Throws<InvalidOperationException>(() => Operation.Create<TestOperation>(configuration));
        }

        [Fact]
        public void You_can_create_a_new_operation_from_an_initialized_operation_instance()
        {
            var resolver = new SimpleOperationResolver();
            var correctConfiguration = new FakeWorkflowConfiguration { Resolver = resolver };
            var sut = new FakeOperation();
            sut.Initialize(correctConfiguration);

            var result = sut.PublicCreate<TestOperation>() as TestOperation;

            Assert.NotNull(result);
        }

        [Theory, AutoMoqData]
        public void You_can_create_a_new_input_operation_from_an_initialized_operation_instance(object input)
        {
            var resolver = new SimpleOperationResolver();
            var correctConfiguration = new FakeWorkflowConfiguration { Resolver = resolver };
            var sut = new FakeOperation();
            sut.Initialize(correctConfiguration);

            var result = sut.PublicCreate<TestInputOperation, object>(input) as TestInputOperation;

            Assert.NotNull(result);
        }

        [Theory, AutoMoqData]
        public void Created_input_operations_are_provided_with_input_values(object input)
        {
            var resolver = new SimpleOperationResolver();
            var correctConfiguration = new FakeWorkflowConfiguration { Resolver = resolver };
            var sut = new FakeOperation();
            sut.Initialize(correctConfiguration);

            var result = sut.PublicCreate<TestInputOperation, object>(input) as TestInputOperation;

            Assert.NotNull(result.InputValue);
            Assert.Equal(input, result.InputValue);
        }

        [Theory, AutoMoqData]
        public void Data_flows_between_child_operations(object output)
        {
            var inputOperation = new FakeInputOperation<object>();
            var outpuOperation = new FakeOutputOperation<object> { OutputValue = output };
            var sut = new FakeOperation(outpuOperation, inputOperation);

            sut.Execute();

            Assert.Equal(outpuOperation.OutputValue, inputOperation.ProvidedInput);
        }

        [Theory, AutoMoqData]
        public void You_can_get_outputted_values_from_within_the_operation(object output)
        {
            var sut = new OutputtingOperation { ExpectedOutput = output };

            sut.Execute();

            Assert.Equal(sut.ExpectedOutput, sut.ActualOutput);
        }

        [Theory, AutoMoqData]
        public void You_can_get_outputted_collection_values_from_within_the_operation(IEnumerable<object> output)
        {
            var sut = new OutputtingCollectionOperation { ExpectedOutput = output };

            sut.Execute();

            Assert.Equal(sut.ExpectedOutput, sut.ActualOutput);
        }

        [Theory, AutoMoqData]
        public void Input_data_automatically_flows_to_child_operations_when_consumed_in_parent_operation(object output)
        {
            var outputOperation = new FakeOutputOperation<object> { OutputValue = output };
            var childInputOperation = new FakeInputOperation<object>();
            var parentInputOperation = new FakeInputOperation<object>(childInputOperation);
            var sut = new FakeOperation(outputOperation, parentInputOperation);

            sut.Execute();

            Assert.Equal(outputOperation.OutputValue, childInputOperation.ProvidedInput);
        }

        [Theory, AutoMoqData]
        public void Input_data_flow_is_cut_off_from_child_operations_if_not_consumed_by_parent_operation(object output)
        {
            var outputOperation = new FakeOutputOperation<object> { OutputValue = output };
            var childInputOperation = new FakeInputOperation<object>();
            var parentInputOperation = new FakeOperation(childInputOperation);
            var sut = new FakeOperation(outputOperation, parentInputOperation);

            sut.Execute();

            Assert.Null(childInputOperation.ProvidedInput);
        }

        [Theory, AutoMoqData]
        public void Executed_child_operations_are_added_to_the_execution_info_list(IOperation childOperation)
        {
            var sut = new FakeOperation(childOperation);

            sut.Execute();

            Assert.Equal(1, sut.ExecutedChildOperations.Count());
            Assert.Equal(childOperation, sut.ExecutedChildOperations.ElementAt(0).Operation);
        }

        [Theory, AutoMoqData]
        public void Child_operation_execution_info_contains_error_info(Exception error)
        {
            var childOperation = new FakeOperation { ThrowOnExecute = error };
            var sut = new FakeOperation(childOperation);

            try { sut.Execute(); }
            catch { }

            Assert.Equal(1, sut.ExecutedChildOperations.Count());
            Assert.Equal(childOperation.ThrowOnExecute, sut.ExecutedChildOperations.ElementAt(0).Error);
        }

        [Fact]
        public void Child_execution_info_contains_the_start_and_end_times_of_the_execution()
        {
            var childOperation = new FakeOperation { ExecuteAction = () => Time.Wait(TimeSpan.FromSeconds(1))};
            var sut = new FakeOperation(childOperation);
            Time.Stop();
            var started = Time.OffsetUtcNow;

            sut.Execute();

            var executionInfo = sut.ExecutedChildOperations.ElementAt(0);
            Assert.Equal(started, executionInfo.Started);
            Assert.Equal(started.AddSeconds(1), executionInfo.Completed);
        }

        private class TestOperation : Operation {
            public WorkflowConfiguration Configuration { get; private set; }

            public override void Initialize(WorkflowConfiguration configuration)
            {
                Configuration = configuration;
            }

            protected override void OnExecute() { }
        }

        private class OutputtingOperation : Operation
        {
            public object ExpectedOutput { get; set; }
            public object ActualOutput { get; private set; }

            protected override void OnExecute() { }

            public override IEnumerable<IOperation> GetChildOperations()
            {
                yield return new FakeOutputOperation<object> { OutputValue = ExpectedOutput };
                ActualOutput = GetChildOutputValue<object>();
            }
        }

        private class OutputtingCollectionOperation : Operation
        {
            public IEnumerable<object> ExpectedOutput { get; set; }
            public IEnumerable<object> ActualOutput { get; private set; }

            protected override void OnExecute() { }

            public override IEnumerable<IOperation> GetChildOperations()
            {
                yield return new FakeOutputOperation<IEnumerable<object>> { OutputValue = ExpectedOutput };
                ActualOutput = GetChildOutputValues<object>();
            }
        }

        private class TestInputOperation : Operation, IInputOperation<object>
        {
            public object InputValue { get; private set; }

            public void Input(object input)
            {
                InputValue = input;
            }

            protected override void OnExecute() { }
        }
    }
}

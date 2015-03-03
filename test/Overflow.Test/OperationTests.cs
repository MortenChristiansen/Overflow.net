using System;
using System.Collections.Generic;
using System.Linq;
using Overflow.Test.Fakes;
using Xunit;

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

        [Fact]
        public void You_can_create_operation_with_a_configuration_containing_a_resolver()
        {
            var correctConfiguration = new FakeWorkflowConfiguration { Resolver = new SimpleOperationResolver() };

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

        [Fact]
        public void You_cannot_create_operations_when_the_operation_resolver_is_not_set()
        {
            Assert.Throws<InvalidOperationException>(() => Operation.Create<TestOperation>(new FakeWorkflowConfiguration()));
        }

        [Fact]
        public void Data_flows_between_child_operations()
        {
            var inputOperation = new FakeInputOperation<object>();
            var outpuOperation = new FakeOutputOperation<object> { OutputValue = new object() };
            var sut = new FakeOperation(outpuOperation, inputOperation);

            sut.Execute();

            Assert.Equal(outpuOperation.OutputValue, inputOperation.ProvidedInput);
        }

        [Fact]
        public void You_can_get_outputted_values_from_within_the_operation()
        {
            var sut = new OutputtingOperation { ExpectedOutput = new object() };

            sut.Execute();

            Assert.Equal(sut.ExpectedOutput, sut.ActualOutput);
        }

        [Fact]
        public void Input_data_automatically_flows_to_child_operations_when_consumed_in_parent_operation()
        {
            var outputOperation = new FakeOutputOperation<object> { OutputValue = new object() };
            var childInputOperation = new FakeInputOperation<object>();
            var parentInputOperation = new FakeInputOperation<object>(childInputOperation);
            var sut = new FakeOperation(outputOperation, parentInputOperation);

            sut.Execute();

            Assert.Equal(outputOperation.OutputValue, childInputOperation.ProvidedInput);
        }

        [Fact]
        public void Input_data_flow_is_cut_off_from_child_operations_if_not_consumed_by_parent_operation()
        {
            var outputOperation = new FakeOutputOperation<object> { OutputValue = new object() };
            var childInputOperation = new FakeInputOperation<object>();
            var parentInputOperation = new FakeOperation(childInputOperation);
            var sut = new FakeOperation(outputOperation, parentInputOperation);

            sut.Execute();

            Assert.Null(childInputOperation.ProvidedInput);
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
    }
}

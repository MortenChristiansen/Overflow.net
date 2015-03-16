using System;
using Overflow.Behaviors;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;
using Xunit.Extensions;

namespace Overflow.Test.Behaviors
{
    public class WorkflowRetryBehaviorFactoryTests
    {
        [Theory, AutoMoqData]
        public void When_workflow_configuration_has_retry_exception_types_a_retry_behavior_is_added_to_every_operation_with_those_exception_types(IOperation operation)
        {
            var configuration = new FakeWorkflowConfiguration().WithGlobalRetryBehavior(3, TimeSpan.FromSeconds(5), typeof(Exception));
            var sut = new WorkflowRetryBehaviorFactory();

            var result = sut.CreateBehaviors(operation, configuration);

            Assert.Equal(1, result.Count);
            Assert.IsType<RetryBehavior>(result[0]);
        }

        [Theory, AutoMoqData]
        public void Retry_behaviors_are_created_with_the_settings_from_the_workflow_configuration(IOperation operation)
        {
            var configuration = new FakeWorkflowConfiguration().WithGlobalRetryBehavior(3, TimeSpan.FromSeconds(5), typeof(Exception));
            var sut = new WorkflowRetryBehaviorFactory();

            var result = sut.CreateBehaviors(operation, configuration);

            var retryBehavior = (RetryBehavior)result[0];
            Assert.Equal(new[] { typeof(Exception) }, retryBehavior.RetryExceptionTypes);
            Assert.Equal(3, retryBehavior.TimesToRetry);
            Assert.Equal(TimeSpan.FromSeconds(5), retryBehavior.RetryDelay);
        }

        [Theory, AutoMoqData]
        public void When_workflow_configuration_does_not_have_retry_exception_types_no_behaviors_are_created(IOperation operation)
        {
            var configuration = new FakeWorkflowConfiguration();
            var sut = new WorkflowRetryBehaviorFactory();

            var result = sut.CreateBehaviors(operation, configuration);

            Assert.Equal(0, result.Count);
        }
    }
}

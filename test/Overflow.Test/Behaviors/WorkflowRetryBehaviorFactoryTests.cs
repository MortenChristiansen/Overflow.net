using System;
using Overflow.Behaviors;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test.Behaviors
{
    public class WorkflowRetryBehaviorFactoryTests
    {
        [Fact]
        public void When_workflow_configuration_has_retry_exception_types_a_retry_behavior_is_added_to_every_operation_with_those_exception_types()
        {
            var configuration = new FakeWorkflowConfiguration().WithGlobalRetryBehavior(3, TimeSpan.FromSeconds(5), typeof(Exception));
            var sut = new WorkflowRetryBehaviorFactory();

            var result = sut.CreateBehaviors(new FakeOperation(), configuration);

            Assert.Equal(1, result.Count);
            Assert.IsType<RetryBehavior>(result[0]);
        }

        [Fact]
        public void Retry_behaviors_are_created_with_the_settings_from_the_workflow_configuration()
        {
            var configuration = new FakeWorkflowConfiguration().WithGlobalRetryBehavior(3, TimeSpan.FromSeconds(5), typeof(Exception));
            var sut = new WorkflowRetryBehaviorFactory();

            var result = sut.CreateBehaviors(new FakeOperation(), configuration);

            var retryBehavior = (RetryBehavior)result[0];
            Assert.Equal(new[] { typeof(Exception) }, retryBehavior.RetryExceptionTypes);
            Assert.Equal(3, retryBehavior.TimesToRetry);
            Assert.Equal(TimeSpan.FromSeconds(5), retryBehavior.RetryDelay);
        }

        [Fact]
        public void When_workflow_configuration_does_not_have_retry_exception_types_no_behaviors_are_created()
        {
            var configuration = new FakeWorkflowConfiguration();
            var sut = new WorkflowRetryBehaviorFactory();

            var result = sut.CreateBehaviors(new FakeOperation(), configuration);

            Assert.Equal(0, result.Count);
        }
    }
}

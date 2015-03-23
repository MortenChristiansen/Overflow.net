using System;
using Overflow.Behaviors;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;
using Xunit.Extensions;

namespace Overflow.Test.Behaviors
{
    public class OperationBehaviorAttributeFactoryTests
    {
        [Theory, AutoMoqData]
        public void Behaviors_are_created_for_the_available_behavior_attributes(WorkflowConfiguration configuration, BehaviorOperation operation)
        {
            var sut = new OperationBehaviorAttributeFactory();

            var result = sut.CreateBehaviors(operation, configuration);

            Assert.Equal(1, result.Count);
            Assert.IsType<FakeOperationBehavior>(result[0]);
        }

        [Theory, AutoMoqData]
        public void No_behaviors_are_created_when_there_are_no_behavior_attributes(WorkflowConfiguration configuration, IOperation operation)
        {
            var sut = new OperationBehaviorAttributeFactory();

            var result = sut.CreateBehaviors(operation, configuration);

            Assert.Equal(0, result.Count);
        }

        [Theory, AutoMoqData]
        public void You_cannot_create_behaviors_without_an_operation(WorkflowConfiguration configuration)
        {
            var sut = new OperationBehaviorAttributeFactory();

            Assert.Throws<ArgumentNullException>(() => sut.CreateBehaviors(null, configuration));
        }

        [FakeOperationBehavior]
        public class BehaviorOperation : Operation { }
    }
}

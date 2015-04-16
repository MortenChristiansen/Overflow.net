using Overflow.Extensibility;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;

namespace Overflow.Test.Extensibility
{
    public class OperationResolverHelperTests
    {
        [Theory, AutoMoqData]
        public void Applying_behaviors_returns_the_original_operation_when_no_behaviors_are_created(WorkflowConfiguration configuration, FakeOperation operation)
        {
            var result = OperationResolverHelper.ApplyBehaviors(operation, configuration);

            Assert.Equal(operation, result);
        }

        [Theory, AutoMoqData]
        public void Behaviors_are_applied_sorted_by_precedence_with_the_higher_precedence_behaviors_on_the_outside_across_factories(WorkflowConfiguration configuration, FakeOperation operation)
        {
            var factory1 = new FakeOperationBehaviorFactory();
            factory1.OperationBehaviors.Add(new FakeOperationBehavior { SetPrecedence = BehaviorPrecedence.StateRecovery });
            factory1.OperationBehaviors.Add(new FakeOperationBehavior { SetPrecedence = BehaviorPrecedence.Logging });
            factory1.OperationBehaviors.Add(new FakeOperationBehavior { SetPrecedence = BehaviorPrecedence.WorkCompensation });
            var factory2 = new FakeOperationBehaviorFactory();
            factory2.OperationBehaviors.Add(new FakeOperationBehavior { SetPrecedence = BehaviorPrecedence.PreRecovery });
            factory2.OperationBehaviors.Add(new FakeOperationBehavior { SetPrecedence = BehaviorPrecedence.Containment });
            configuration.WithBehaviorFactory(factory1).WithBehaviorFactory(factory2);

            var result = OperationResolverHelper.ApplyBehaviors(operation, configuration);

            Assert.IsType<FakeOperationBehavior>(result);
            var behavior1 = (OperationBehavior)result;
            Assert.Equal(BehaviorPrecedence.Logging, behavior1.Precedence);
            Assert.IsType<FakeOperationBehavior>(behavior1.InnerOperation);
            var behavior2 = (OperationBehavior)behavior1.InnerOperation;
            Assert.Equal(BehaviorPrecedence.Containment, behavior2.Precedence);
            Assert.IsType<FakeOperationBehavior>(behavior2.InnerOperation);
            var behavior3 = (OperationBehavior)behavior2.InnerOperation;
            Assert.Equal(BehaviorPrecedence.WorkCompensation, behavior3.Precedence);
            Assert.IsType<FakeOperationBehavior>(behavior3.InnerOperation);
            var behavior4 = (OperationBehavior)behavior3.InnerOperation;
            Assert.Equal(BehaviorPrecedence.StateRecovery, behavior4.Precedence);
            Assert.IsType<FakeOperationBehavior>(behavior4.InnerOperation);
            var behavior5 = (OperationBehavior)behavior4.InnerOperation;
            Assert.Equal(BehaviorPrecedence.PreRecovery, behavior5.Precedence);
            Assert.IsType<FakeOperation>(behavior5.InnerOperation);
        }
    }
}

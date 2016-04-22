using System;
using System.Collections.Generic;
using Overflow.Behaviors;
using Overflow.Test.Fakes;
using Overflow.Testing;
using Xunit;

namespace Overflow.Test.Testing
{
    public class FakeOperationResolverTests
    {
        public FakeOperationResolverTests()
        {
            ChildOperation.ExecutedType = null;
        }

        [Fact]
        public void You_can_create_a_fake_operation_resolver()
        {
            new FakeOperationResolver();
        }

        [Fact]
        public void You_can_resolve_a_registered_fake_operation()
        {
            var resolver = new FakeOperationResolver();
            var fakeOperation = new FakeChildOperation();
            resolver.ProvideFakeOperation<ChildOperation>(fakeOperation);

            var result = resolver.Resolve<ChildOperation>(new FakeWorkflowConfiguration());

            Assert.NotNull(result);
            Assert.IsType<FakeChildOperation>(result);
        }

        [Fact]
        public void You_cannot_register_a_fake_operation_implementation_with_the_same_type_as_the_operation_it_is_registered_for()
        {
            var resolver = new FakeOperationResolver();
            Assert.Throws<ArgumentException>(() => resolver.ProvideFakeOperation<ChildOperation>(new ChildOperation()));
        }

        [Fact]
        public void If_you_do_not_register_a_fake_operation_implementation_the_original_type_is_resolved_as_normal()
        {
            var resolver = new FakeOperationResolver();

            var result = resolver.Resolve<ChildOperation>(new FakeWorkflowConfiguration());

            Assert.NotNull(result);
            Assert.IsType<ChildOperation>(result);
        }

        [Fact]
        public void If_you_do_register_multiple_fake_operation_implementations_for_the_same_type_the_last_registration_wins()
        {
            var resolver = new FakeOperationResolver();
            resolver.ProvideFakeOperation<ChildOperation>(new FakeChildOperation());
            resolver.ProvideFakeOperation<ChildOperation>(new FakeChildOperation2());

            var result = resolver.Resolve<ChildOperation>(new FakeWorkflowConfiguration());

            Assert.NotNull(result);
            Assert.IsType<FakeChildOperation2>(result);
        }

        [Fact]
        public void By_default_fake_operations_do_not_get_behaviors_applied()
        {
            var resolver = new FakeOperationResolver();
            resolver.ProvideFakeOperation<ChildOperation>(new FakeChildOperation());

            var result = resolver.Resolve<ChildOperation>(new FakeWorkflowConfiguration().WithBehaviorFactory(new OperationBehaviorAttributeFactory()));

            Assert.IsType<FakeChildOperation>(result);
        }

        [Fact]
        public void You_can_make_resolved_fake_operations_have_behaviors_applied()
        {
            var resolver = new FakeOperationResolver(applyBehaviors: true);
            resolver.ProvideFakeOperation<ChildOperation>(new FakeChildOperation());

            var result = resolver.Resolve<ChildOperation>(new FakeWorkflowConfiguration().WithBehaviorFactory(new OperationBehaviorAttributeFactory()));

            Assert.IsType<AtomicBehavior>(result);
        }

        [Fact]
        public void You_can_supply_a_custom_fallback_resolver_for_when_trying_to_resolve_types_with_no_registered_fakes()
        {
            var innerResolver = new FakeOperationResolver();
            innerResolver.ProvideFakeOperation<ChildOperation>(new FakeChildOperation());
            var resolver = new FakeOperationResolver(innerResolver);

            var result = resolver.Resolve<ChildOperation>(new FakeWorkflowConfiguration());

            Assert.IsType<FakeChildOperation>(result);
        }

        class ParentOperation : Operation
        {
            public override IEnumerable<IOperation> GetChildOperations()
            {
                yield return Create<ChildOperation>();
            }
        }

        class ChildOperation : Operation
        {
            [ThreadStatic]
            public static Type ExecutedType;

            protected override void OnExecute() =>
                ExecutedType = typeof(ChildOperation);
        }

        [Atomic]
        class FakeChildOperation : ChildOperation
        {
            protected override void OnExecute() =>
                ExecutedType = typeof(FakeChildOperation);
        }

        class FakeChildOperation2 : ChildOperation
        {
            protected override void OnExecute() =>
                ExecutedType = typeof(FakeChildOperation2);
        }
    }
}

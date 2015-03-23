using System;
using Overflow.Extensibility;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;
using Xunit.Extensions;

namespace Overflow.Test
{
    public class SimpleOperationResolverTests
    {
        [Theory, AutoMoqData]
        public void The_simple_resolver_can_resolve_operations_without_any_dependencies(WorkflowConfiguration configuration)
        {
            var sut = new SimpleOperationResolver();

            var result = sut.Resolve<SimpleTestOperation>(configuration);

            Assert.NotNull(result);
        }

        [Theory, AutoMoqData]
        public void The_simple_resolver_can_resolve_operations_with_registered_dependencies(WorkflowConfiguration configuration)
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();

            var result = sut.Resolve<OperationWithDependencies>(configuration);

            Assert.NotNull(result);
        }

        [Theory, AutoMoqData]
        public void You_can_register_the_same_dependency_more_than_once(WorkflowConfiguration configuration)
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();

            var result = sut.Resolve<OperationWithDependencies>(configuration);

            Assert.NotNull(result);
        }

        [Theory, AutoMoqData]
        public void The_simple_resolver_can_resolve_operations_with_nested_dependencies(WorkflowConfiguration configuration)
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();
            sut.RegisterOperationDependency<ComplexDependency, ComplexDependency>();

            var result = sut.Resolve<OperationWithComplexDependencies>(configuration);

            Assert.NotNull(result);
        }

        [Theory, AutoMoqData]
        public void The_simple_resolver_cannot_resolve_operations_with_unregistered_dependencies(WorkflowConfiguration configuration)
        {
            var sut = new SimpleOperationResolver();

            Assert.Throws<InvalidOperationException>(() => sut.Resolve<OperationWithDependencies>(configuration));
        }

        [Theory, AutoMoqData]
        public void The_simple_resolver_cannot_resolve_operations_with_unregistered_sub_dependencies(WorkflowConfiguration configuration)
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<ComplexDependency, ComplexDependency>();

            Assert.Throws<InvalidOperationException>(() => sut.Resolve<OperationWithComplexDependencies>(configuration));
        }

        [Theory, AutoMoqData]
        public void Resolving_the_same_operation_twice_returns_two_different_instances(WorkflowConfiguration configuration)
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();

            var result1 = sut.Resolve<SimpleTestOperation>(configuration);
            var result2 = sut.Resolve<SimpleTestOperation>(configuration);

            Assert.NotSame(result1, result2);
        }

        [Theory, AutoMoqData]
        public void Dependencies_are_resolved_as_new_instances_every_time(WorkflowConfiguration configuration)
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();

            var result1 = sut.Resolve<OperationWithDependencies>(configuration) as OperationWithDependencies;
            var result2 = sut.Resolve<OperationWithDependencies>(configuration) as OperationWithDependencies;

            Assert.NotSame(result1.Dependency, result2.Dependency);
        }

        [Theory, AutoMoqData]
        public void Operations_having_more_than_one_constructor_cannot_be_resolved(WorkflowConfiguration configuration)
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();

            Assert.Throws<InvalidOperationException>(() => sut.Resolve<OperationWithTwoConstructors>(configuration));
        }

        [Theory, AutoMoqData]
        public void Operations_with_dependencies_having_more_than_one_constructor_cannot_be_resolved(WorkflowConfiguration configuration)
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();
            sut.RegisterOperationDependency<DependencyWithTwoConstructors, DependencyWithTwoConstructors>();

            Assert.Throws<InvalidOperationException>(() => sut.Resolve<OperationWithDualConstructorDependency>(configuration));
        }

        [Theory, AutoMoqData]
        public void Dependencies_can_be_registered_as_implementations(WorkflowConfiguration configuration)
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<IDependency, SimpleDependency>();

            var result = sut.Resolve<OperationWithInterfaceDependency>(configuration);

            Assert.NotNull(result);
        }

        [Theory, AutoMoqData]
        public void The_last_registered_dependency_of_a_given_type_wins(WorkflowConfiguration configuration)
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();
            sut.RegisterOperationDependency<IDependency, SimpleDependency>();
            sut.RegisterOperationDependency<IDependency, ComplexDependency>();

            var result = sut.Resolve<OperationWithInterfaceDependency>(configuration) as OperationWithInterfaceDependency;

            Assert.IsType<ComplexDependency>(result.Dependency);
        }

        [Theory, AutoMoqData]
        public void Resolving_operations_creates_and_applies_behaviors_to_the_created_operations(WorkflowConfiguration configuration)
        {
            var sut = new SimpleOperationResolver();
            var factory = new FakeOperationBehaviorFactory();
            factory.OperationBehaviors.Add(new FakeOperationBehavior { SetPrecedence = BehaviorPrecedence.StateRecovery });
            var workflow = configuration.WithBehaviorFactory(factory);

            var result = sut.Resolve<SimpleTestOperation>(workflow);

            Assert.IsType<FakeOperationBehavior>(result);
            Assert.IsType<SimpleTestOperation>((result as OperationBehavior).InnerOperation);
        }

        #region Dependencies

        private class SimpleTestOperation : Operation
        {
            protected override void OnExecute() { }
        }

        private class OperationWithDependencies : Operation
        {
            public SimpleDependency Dependency { get; private set; }

            public OperationWithDependencies(SimpleDependency dependency)
            {
                if (dependency == null) throw new ArgumentException();
                Dependency = dependency;
            }

            protected override void OnExecute() { }
        }

        private interface IDependency { }

        private class SimpleDependency : IDependency { }

        private class OperationWithComplexDependencies : Operation
        {
            public ComplexDependency Dependency { get; private set; }

            public OperationWithComplexDependencies(ComplexDependency dependency)
            {
                if (dependency == null) throw new ArgumentException();
                Dependency = dependency;
            }

            protected override void OnExecute() { }
        }

        private class ComplexDependency : IDependency
        {
            public SimpleDependency Dependency { get; private set; }

            public ComplexDependency(SimpleDependency dependency)
            {
                if (dependency == null) throw new ArgumentException();
                Dependency = dependency;
            }
        }

        private class OperationWithDualConstructorDependency : Operation
        {
            public OperationWithDualConstructorDependency(DependencyWithTwoConstructors dependency)
            {
                if (dependency == null) throw new ArgumentException();
            }

            protected override void OnExecute() { }
        }

        private class DependencyWithTwoConstructors : IDependency
        {
            public DependencyWithTwoConstructors() { }
            public DependencyWithTwoConstructors(SimpleDependency dependency) { }
        }

        private class OperationWithTwoConstructors : Operation
        {
            public OperationWithTwoConstructors() { }
            public OperationWithTwoConstructors(SimpleDependency dependency) { }

            protected override void OnExecute() { }
        }

        private class OperationWithInterfaceDependency : Operation
        {
            public IDependency Dependency { get; private set; }

            public OperationWithInterfaceDependency(IDependency dependency)
            {
                if (dependency == null) throw new ArgumentException();
                Dependency = dependency;
            }

            protected override void OnExecute() { }
        }

        #endregion
    }
}

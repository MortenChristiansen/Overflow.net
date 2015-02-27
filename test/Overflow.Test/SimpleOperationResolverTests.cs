using System;
using Xunit;

namespace Overflow.Test
{
    public class SimpleOperationResolverTests
    {
        [Fact]
        public void The_simple_resolver_can_resolve_operations_without_any_dependencies()
        {
            var sut = new SimpleOperationResolver();

            var result = sut.Resolve<SimpleTestOperation>();

            Assert.NotNull(result);
        }

        [Fact]
        public void The_simple_resolver_can_resolve_operations_with_registered_dependencies()
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();

            var result = sut.Resolve<OperationWithDependencies>();

            Assert.NotNull(result);
        }

        [Fact]
        public void You_can_Register_the_same_dependency_more_than_once()
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();

            var result = sut.Resolve<OperationWithDependencies>();

            Assert.NotNull(result);
        }

        [Fact]
        public void The_simple_resolver_can_resolve_operations_with_nested_dependencies()
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();
            sut.RegisterOperationDependency<ComplexDependency, ComplexDependency>();

            var result = sut.Resolve<OperationWithComplexDependencies>();

            Assert.NotNull(result);
        }

        [Fact]
        public void The_simple_resolver_cannot_resolve_operations_with_unregistered_dependencies()
        {
            var sut = new SimpleOperationResolver();

            Assert.Throws<InvalidOperationException>(() => sut.Resolve<OperationWithDependencies>());
        }

        [Fact]
        public void The_simple_resolver_cannot_resolve_operations_with_unregistered_sub_dependencies()
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<ComplexDependency, ComplexDependency>();

            Assert.Throws<InvalidOperationException>(() => sut.Resolve<OperationWithComplexDependencies>());
        }

        [Fact]
        public void Resolving_the_same_operation_twice_returns_two_different_instances()
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();

            var result1 = sut.Resolve<SimpleTestOperation>();
            var result2 = sut.Resolve<SimpleTestOperation>();

            Assert.NotSame(result1, result2);
        }

        [Fact]
        public void Dependencies_are_resolved_as_new_instances_every_time()
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();

            var result1 = sut.Resolve<OperationWithDependencies>() as OperationWithDependencies;
            var result2 = sut.Resolve<OperationWithDependencies>() as OperationWithDependencies;

            Assert.NotSame(result1.Dependency, result2.Dependency);
        }

        [Fact]
        public void Operations_having_more_than_one_constructor_cannot_be_resolved()
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();

            Assert.Throws<InvalidOperationException>(() => sut.Resolve<OperationWithTwoConstructors>());
        }

        [Fact]
        public void Operations_with_dependencies_having_more_than_one_constructor_cannot_be_resolved()
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();
            sut.RegisterOperationDependency<DependencyWithTwoConstructors, DependencyWithTwoConstructors>();

            Assert.Throws<InvalidOperationException>(() => sut.Resolve<OperationWithDualConstructorDependency>());
        }

        [Fact]
        public void Dependencies_can_be_registered_as_implementations()
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<IDependency, SimpleDependency>();

            var result = sut.Resolve<OperationWithInterfaceDependency>();

            Assert.NotNull(result);
        }

        [Fact]
        public void The_last_registered_dependency_of_a_given_type_wins()
        {
            var sut = new SimpleOperationResolver();
            sut.RegisterOperationDependency<SimpleDependency, SimpleDependency>();
            sut.RegisterOperationDependency<IDependency, SimpleDependency>();
            sut.RegisterOperationDependency<IDependency, ComplexDependency>();

            var result = sut.Resolve<OperationWithInterfaceDependency>() as OperationWithInterfaceDependency;

            Assert.IsType<ComplexDependency>(result.Dependency);
        }

        [Fact]
        public void Operations_with_retry_on_error_attribute_are_decorated_with_retry_decorator()
        {
            var sut = new SimpleOperationResolver();

            var result = sut.Resolve<RetryOnFailureOperation>();

            Assert.IsType<RetryOnFailureOperationDecorator>(result);
            Assert.IsType<RetryOnFailureOperation>((result as OperationDecorator).DecoratedOperation);
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

        [RetryOnFailure]
        private class RetryOnFailureOperation : Operation
        {
            protected override void OnExecute() { }
        }

        #endregion
    }
}

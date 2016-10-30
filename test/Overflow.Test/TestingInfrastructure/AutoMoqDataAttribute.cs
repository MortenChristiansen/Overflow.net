using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Overflow.Test.Fakes;
using Xunit.Sdk;

namespace Overflow.Test.TestingInfrastructure
{
    class AutoMoqDataAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var specimens = new List<object>();
            foreach (var p in testMethod.GetParameters())
            {
                var specimen = Resolve(p.ParameterType);
                specimens.Add(specimen);
            }

            return new[] { specimens.ToArray() };
        }

        private static readonly Random _rnd = new Random();

        private static object Resolve(Type type)
        {
            if (type == typeof(string))
                return Guid.NewGuid().ToString();

            if (type == typeof(int))
                return _rnd.Next();

            if (type == typeof(decimal))
                return (decimal)_rnd.NextDouble();

            if (type == typeof(double))
                return _rnd.NextDouble();

            if (type == typeof(float))
                return (float)_rnd.NextDouble();

            if (type == typeof(DateTime))
                return DateTime.Now.AddDays(_rnd.Next(-1000, 1000)).AddMinutes(_rnd.Next(-1000, 1000));

            if (type == typeof(DateTimeOffset))
                return DateTimeOffset.Now.AddDays(_rnd.Next(-1000, 1000)).AddMinutes(_rnd.Next(-1000, 1000));

            if (type == typeof(object))
                return new object();

            if (type == typeof(bool))
                return _rnd.Next() % 2 == 0;

            if (type == typeof(Guid))
                return Guid.NewGuid();

            if (type == typeof(IOperation))
                return new FakeOperation();
            
            var typeInfo = type.GetTypeInfo();
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly);
            var defaultConstructor = constructors.FirstOrDefault(c => !c.GetParameters().Any());
            if (defaultConstructor != null && !typeInfo.IsAbstract)
                return defaultConstructor.Invoke(null);

            var generator = new ProxyGenerator();
            if (typeInfo.IsArray)
                return CreateArray(typeInfo.GetElementType(), 3);

            if (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return CreateArray(type.GenericTypeArguments[0], 3);

            if (typeInfo.IsInterface)
                return generator.CreateInterfaceProxyWithoutTarget(type, new DummyInterceptor());

            if (typeInfo.IsAbstract)
                return generator.CreateClassProxy(type, new DummyInterceptor());

            if (typeInfo.IsSealed)
                throw new NotSupportedException($"Cannot create instance of sealed class {type.Name}. It has no default constructor.");

            var constructor = constructors.OrderBy(c => c.GetParameters().Length).FirstOrDefault();
            if (constructor != null)
            {
                var parameters = constructor.GetParameters().Select(p => Resolve(p.ParameterType)).ToArray();
                return constructor.Invoke(parameters);
            }

            return generator.CreateClassProxy(type, new DummyInterceptor());
        }

        private static object CreateArray(Type elementType, int items)
        {
            var array = Array.CreateInstance(elementType, items);

            for (var i = 0; i < items; i++)
                array.SetValue(Resolve(elementType), i);
            
            return array;
        }

        class DummyInterceptor : IInterceptor
        {
            public void Intercept(IInvocation invocation)
            {
                if (invocation.Method.ReturnType != null)
                    invocation.ReturnValue = Resolve(invocation.Method.ReturnType);
            }
        }
    }
}

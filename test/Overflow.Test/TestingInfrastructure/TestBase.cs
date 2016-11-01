using System;
using System.Reflection;
using Overflow.Testing;
using Xunit;

namespace Overflow.Test.TestingInfrastructure
{
    public abstract class TestBase
    {
        protected static readonly string NL = Environment.NewLine;

        protected void VerifyConstructorGuards()
        {
            /*var assertion = new GuardClauseAssertion(Fixture);
            assertion.Verify(GetType().GetConstructors());*/
        }

        protected void VerifyMethodGuards<TSut>()
        {
            foreach (var method in typeof(TSut).GetTypeInfo().GetMethods())
                Verify(method);
        }

        private static void Verify(MethodInfo method)
        {
            // Do not test the methods on object
            if (method.DeclaringType.Name.Equals("object", StringComparison.OrdinalIgnoreCase))
                return;

            var parameters = method.GetParameters();
            // If there are no parameters, there is nothing to test
            if (parameters.Length == 0)
                return;

            // If the method is generic, we need to bind it to a concrete type
            bool genericArgumentIsReferenceType;
            Type genericType;
            method = BindGenericMethod(method, out genericArgumentIsReferenceType, out genericType);

            for (var i = 0; i < parameters.Length; i++)
                VerifyParameterNullCheck(method, parameters, i, genericArgumentIsReferenceType, genericType);
        }

        private static MethodInfo BindGenericMethod(MethodInfo method, out bool genericArgumentIsReferenceType, out Type genericType)
        {
            genericType = null;
            genericArgumentIsReferenceType = false;
            if (!method.IsGenericMethod)
                return method;

            var genericArguments = method.GetGenericArguments();
            if (genericArguments.Length != 1)
                throw new NotSupportedException("Only implemented for 1 generic argument");

            genericType = genericArguments[0];
            var constraints = genericType.GetTypeInfo().GenericParameterAttributes;
            genericArgumentIsReferenceType = constraints.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint);
            return method.MakeGenericMethod(typeof(object));
        }

        private static void VerifyParameterNullCheck(MethodInfo method, ParameterInfo[] parameters, int parameterIndex, bool genericArgumentIsReferenceType, Type genericType)
        {
            var param = parameters[parameterIndex];
            if (param.ParameterType.GetTypeInfo().IsValueType)
                return;

            if (!genericArgumentIsReferenceType && param.ParameterType == genericType)
                return;

            var parameterObjects = PopulateMethodParameters(parameters, parameterIndex);
            var instance = DataResolver.Resolve(method.DeclaringType);
            VerifyMethodInvoke(method, instance, parameterObjects, param);
        }

        private static object[] PopulateMethodParameters(ParameterInfo[] parameters, int parameterIndex)
        {
            var parameterObjects = new object[parameters.Length];
            for (var j = 0; j < parameterObjects.Length; j++)
                parameterObjects[j] = j == parameterIndex ? null : DataResolver.Resolve(parameters[j].ParameterType);
            return parameterObjects;
        }

        private static void VerifyMethodInvoke(MethodInfo method, object instance, object[] parameterObjects, ParameterInfo param)
        {
            try
            {
                method.Invoke(instance, parameterObjects);
            }
            catch (TargetInvocationException e)
            {
                Assert.IsType(typeof(ArgumentNullException), e.InnerException);
                return;
            }

            throw new AssertionException($"Method '{method.Name}' on type '{method.DeclaringType.Name}' did not test for null value for parameter '{param.Name}'");
        }

        protected void VerifyGuards<TSut>()
        {
            VerifyConstructorGuards();
            VerifyMethodGuards<TSut>();
        }

        protected void ExecuteIgnoringErrors(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
            }
        }
    }
}

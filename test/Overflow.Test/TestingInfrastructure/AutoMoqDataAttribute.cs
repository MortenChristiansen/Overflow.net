using System.Collections.Generic;
using System.Reflection;
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
                var specimen = DataResolver.Resolve(p.ParameterType);
                specimens.Add(specimen);
            }

            return new[] { specimens.ToArray() };
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Overflow.Extensibility;
using Overflow.Utilities;

namespace Overflow.Behaviors
{
    class OperationBehaviorAttributeFactory : IOperationBehaviorFactory
    {
        public IList<OperationBehavior> CreateBehaviors(IOperation operation, WorkflowConfiguration configuration)
        {
            Verify.NotNull(operation, nameof(operation));

            var decoratorAttributes = operation.GetType().GetCustomAttributes(typeof(OperationBehaviorAttribute), inherit: false);
            return decoratorAttributes.OfType<OperationBehaviorAttribute>().Select(b => b.CreateBehavior(configuration)).ToList();
        }
    }
}

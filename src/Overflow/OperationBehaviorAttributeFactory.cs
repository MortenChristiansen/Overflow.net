using System;
using System.Collections.Generic;
using System.Linq;

namespace Overflow
{
    class OperationBehaviorAttributeFactory : IOperationBehaviorFactory
    {
        public IList<OperationBehavior> CreateBehaviors(IOperation operation, WorkflowConfiguration configuration)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            var decoratorAttributes = operation.GetType().GetCustomAttributes(typeof(OperationBehaviorAttribute), inherit: false);
            return decoratorAttributes.OfType<OperationBehaviorAttribute>().Select(b => b.CreateBehavior()).ToList();
        }
    }
}

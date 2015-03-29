using System.Collections.Generic;
using CustomOperationResolver.DomainClasses;
using Overflow;

namespace CustomOperationResolver.Operations
{
    /// <summary>
    /// A workflow that takes all the users which have not yet had their
    /// location identified and looks it up using an online location service.
    /// 
    /// The UpdateUserLocationOperation is marked with Retry behavior, which
    /// means that if there are any errors connecting to the service, it will
    /// retry three times before giving up, failing the operation (and the
    /// entire workflow).
    /// </summary>
    class RetrieveUserLocationsWorkflow : Operation
    {
        public override IEnumerable<IOperation> GetChildOperations()
        {
            yield return Create<GetUsersWithoutLocationOperation>();

            // Example note: This is a case where you might want to wrap the two operations
            // in a single parent operation. This would allow you to apply behaviors to them
            // together, such as the ContinueOnFailure behavior, which could make sense in
            // this case.
            foreach (var user in GetChildOutputValues<User>())
            {
                yield return Create<UpdateUserLocationOperation, User>(user);
                yield return Create<SaveUserLocationOperation, User>(user);
            }
        }
    }
}

using System.Collections.Generic;
using ConsoleWorkflows.DomainClasses;
using Overflow;

namespace ConsoleWorkflows.Workflows.SendTaskNotifications
{
    /// <summary>
    /// A workflow that checks if there have been defined any tasks
    /// for users, which have not been completed and which has not
    /// already had notifications sent for them.
    /// 
    /// For each task, it sends a corresponding notification and
    /// marks the task as having been notified so it will not be
    /// notified on in the future.
    /// 
    /// The SendNotificationOperation is marked as ContinueOnError
    /// which means that if one email cannot be sent, it does not 
    /// interfere with the rest.
    /// 
    /// This is a simple workflow that illustrates how data flows
    /// between operations as well as simple behavior usage.
    /// </summary>
    class SendTaskNotificationsWorkflow : Operation
    {
        protected override void OnExecute() { }

        public override IEnumerable<IOperation> GetChildOperations()
        {
            yield return Create<CreateTaskNotificationsOperation>();
            foreach (var notification in GetChildOutputValues<Notification>())
                yield return Create<SendNotificationOperation, Notification>(notification);
        }
    }
}

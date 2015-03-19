using ConsoleWorkflows.DomainClasses;
using Overflow;

namespace ConsoleWorkflows.Workflows.SendTaskNotifications
{
    class SendNotificationOperation : Operation, IInputOperation<Notification>
    {
        protected override void OnExecute()
        {
            throw new System.NotImplementedException();
        }

        public void Input(Notification input)
        {
            throw new System.NotImplementedException();
        }
    }
}

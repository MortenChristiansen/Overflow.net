using System.Collections.Generic;
using ConsoleWorkflows.DomainClasses;
using Overflow;

namespace ConsoleWorkflows.Workflows.SendTaskNotifications
{
    class SendTaskNotificationsWorkflow : Operation
    {
        private readonly TaskRepository _taskRepository;

        public SendTaskNotificationsWorkflow(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        protected override void OnExecute()
        {
            var tasks = _taskRepository.GetUnfinishedAndUnnotifiedTasks();
        }

        public override IEnumerable<IOperation> GetChildOperations()
        {
            //yield return Create<CreateTaskNotifications>();
            //foreach (var notification in GetChildOutput<IEnumerable<Notification>>());
            //    yield return Create<SendNotificationOperation>(notification);

            yield break;
        }
    }
}

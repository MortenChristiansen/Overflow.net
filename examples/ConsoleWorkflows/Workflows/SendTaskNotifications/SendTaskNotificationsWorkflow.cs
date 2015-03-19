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
            yield return Create<CreateTaskNotificationsOperation>();
            foreach (var notification in GetChildOutputValue<IEnumerable<Notification>>())
                yield return Create<SendNotificationOperation, Notification>(notification);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using ConsoleWorkflows.DomainClasses;
using Overflow;

namespace ConsoleWorkflows.Operations
{
    class CreateTaskNotificationsOperation : Operation
    {
        private readonly TaskRepository _taskRepository;

        [Output]
        public IEnumerable<Notification> Notifications { get; private set; }

        public CreateTaskNotificationsOperation(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        protected override void OnExecute()
        {
            var tasks = _taskRepository.GetUnfinishedAndUnnotifiedTasks();
            Notifications = tasks.Select(t => new Notification(t));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleWorkflows.DomainClasses;
using Overflow;

namespace ConsoleWorkflows.Operations
{
    class CreateTaskNotificationsOperation : Operation, IOutputOperation<IEnumerable<Notification>>
    {
        private readonly TaskRepository _taskRepository;
        private Action<IEnumerable<Notification>> _outputNotifications;

        public CreateTaskNotificationsOperation(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public void Output(Action<IEnumerable<Notification>> onReceiveOutput)
        {
            _outputNotifications = onReceiveOutput;
        }

        protected override void OnExecute()
        {
            var tasks = _taskRepository.GetUnfinishedAndUnnotifiedTasks();
            var notifications = tasks.Select(t => new Notification(t));
            _outputNotifications(notifications);
        }
    }
}

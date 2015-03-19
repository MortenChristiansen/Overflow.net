using System.Collections.Generic;

namespace ConsoleWorkflows.DomainClasses
{
    class TaskRepository
    {
        public IEnumerable<Task> GetUnfinishedAndUnnotifiedTasks()
        {
            return new Task[0];
        }
    }
}

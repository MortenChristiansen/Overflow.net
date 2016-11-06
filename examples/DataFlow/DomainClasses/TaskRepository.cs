using System.Collections.Generic;

namespace DataFlow.DomainClasses
{
    class TaskRepository
    {
        public IEnumerable<Task> GetUnfinishedAndUnnotifiedTasks()
        {
            return new []{ new Task(), new Task(), new Task() };
        }

        public void MarkTaskAsNotified(Task task)
        {
            
        }
    }
}

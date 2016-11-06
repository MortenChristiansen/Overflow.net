using DataFlow.DomainClasses;

namespace ConsoleWorkflows.DomainClasses
{
    class Notification
    {
        public string RecipientEmail { get; private set; }
        public string Topic { get; private set; }
        public string Content { get; private set; }
        public Task Task { get; private set; }

        public Notification(Task task)
        {
            Task = task;
        }

        public MailMessage ToMailMessage()
        {
            return new MailMessage();
        }
    }
}

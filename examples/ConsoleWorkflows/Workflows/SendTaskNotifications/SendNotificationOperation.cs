using ConsoleWorkflows.DomainClasses;
using Overflow;

namespace ConsoleWorkflows.Workflows.SendTaskNotifications
{
    [ContinueOnFailure]
    class SendNotificationOperation : Operation, IInputOperation<Notification>
    {
        private readonly TaskRepository _taskRepository;
        private readonly EmailService _emailService;

        private Notification _notification;

        public SendNotificationOperation(TaskRepository taskRepository, EmailService emailService)
        {
            _taskRepository = taskRepository;
            _emailService = emailService;
        }

        public void Input(Notification input)
        {
            _notification = input;
        }

        protected override void OnExecute()
        {
            var mail = _notification.ToMailMessage();
            _emailService.Send(mail);

            // Example note: This might as well have been done in its own operation. It is just a matter of taste
            // and how fine grained control you need. One reason for splitting them up is if you want to add a retry
            // behavior to the sending of the emails, because you feel it is error prone. You don't want to accidentally
            // resend any emails because the database update failed.
            _taskRepository.MarkTaskAsNotified(_notification.Task);
        }
    }
}

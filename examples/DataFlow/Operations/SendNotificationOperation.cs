using ConsoleWorkflows.DomainClasses;
using Overflow;

namespace ConsoleWorkflows.Operations
{
    [ContinueOnFailure]
    class SendNotificationOperation : Operation
    {
        private readonly TaskRepository _taskRepository;
        private readonly EmailService _emailService;

        [Input]
        public Notification Notification { get; set; }

        public SendNotificationOperation(TaskRepository taskRepository, EmailService emailService)
        {
            _taskRepository = taskRepository;
            _emailService = emailService;
        }

        protected override void OnExecute()
        {
            var mail = Notification.ToMailMessage();
            _emailService.Send(mail);

            // Example note: This might as well have been done in its own operation. It is just a matter of taste
            // and how fine grained control you need. One reason for splitting them up is if you want to add a retry
            // behavior to the sending of the emails, because you feel it is error prone. You don't want to accidentally
            // resend any emails because the database update failed.
            _taskRepository.MarkTaskAsNotified(Notification.Task);
        }
    }
}

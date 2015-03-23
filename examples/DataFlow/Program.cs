using System;
using ConsoleWorkflows.DomainClasses;
using ConsoleWorkflows.Workflows.SendTaskNotifications;
using Overflow;

namespace ConsoleWorkflows
{
    class Program
    {
        static void Main(string[] args)
        {
            var resolver = new SimpleOperationResolver();
            resolver.RegisterOperationDependency<TaskRepository, TaskRepository>();
            resolver.RegisterOperationDependency<EmailService, EmailService>();

            var sendTaskNotifications = Workflow.Configure<SendTaskNotificationsWorkflow>().
                WithLogger(new TextWriterWorkflowLogger(Console.Out)).
                WithResolver(resolver).
                CreateOperation();

            sendTaskNotifications.Execute();

            Console.WriteLine();
            Console.WriteLine("Press eny key to exit");
            Console.ReadLine();
        }
    }
}

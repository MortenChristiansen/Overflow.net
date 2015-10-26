using Compact.Operations;
using Compact.Services;
using Overflow;
using System;

namespace Compact
{
    class Program
    {
        static void Main(string[] args)
        {
            var resolver = new SimpleOperationResolver();
            resolver.RegisterOperationDependencyInstance<Func<string, IExternalDatabase>>(connectionString => new ExternalDatabase());
            resolver.RegisterOperationDependency<IDatabase, Database>();
            resolver.RegisterOperationDependency<IEmailService, EmailService>();
            resolver.RegisterOperationDependency<ISmsService, SmsService>();

            var importUsers = Workflow.Configure<ImportUsersWorkflow>()
                .WithLogger(new TextWriterWorkflowLogger(Console.Out))
                .WithResolver(resolver)
                .CreateOperation();

            importUsers.Execute();

            Console.WriteLine();
            Console.WriteLine("Press eny key to exit");
            Console.ReadLine();
        }
    }
}

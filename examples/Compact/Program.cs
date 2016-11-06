using Compact.Operations;
using Compact.Services;
using Overflow;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Compact
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> { { "ExternalDbConnectionString", "some connection string" } }).Build();

                      var resolver = new SimpleOperationResolver();
            resolver.RegisterOperationDependencyInstance<Func<string, IExternalDatabase>>(connectionString => new ExternalDatabase());
            resolver.RegisterOperationDependencyInstance<IConfiguration>(configuration);
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

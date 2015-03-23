using System;
using Autofac;
using CustomOperationResolver.DomainClasses;
using CustomOperationResolver.Extensions;
using CustomOperationResolver.Operations;
using Overflow;

namespace CustomOperationResolver
{
    class Program
    {
        static void Main(string[] args)
        {
            var autofacResolver = CreateResolver();

            var updateUserLocations = Workflow.Configure<RetrieveUserLocationsWorkflow>().
               WithLogger(new TextWriterWorkflowLogger(Console.Out)).
               WithResolver(autofacResolver).
               CreateOperation();

            updateUserLocations.Execute();

            Console.WriteLine();
            Console.WriteLine("Press eny key to exit");
            Console.ReadLine();
        }

        private static AutofacOperationResolver CreateResolver()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new UserLocationLookupService());
            builder.RegisterInstance(new UserRepository());
            var container = builder.Build();
            var autofacResolver = new AutofacOperationResolver(container);
            return autofacResolver;
        }
    }
}

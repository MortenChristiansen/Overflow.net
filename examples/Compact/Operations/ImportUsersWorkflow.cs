using System;
using Compact.DomainClasses;
using Compact.Services;
using Overflow;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Compact.Operations
{
    /// <summary>
    /// A workflow that takes all the users from an external database which have not
    /// yet been imported and imports them into the local database. On successful 
    /// import, a notification is sent to each user.
    /// 
    /// The database operations are marked with Retry behavior, which
    /// means that if there are any errors connecting to the databases, they will
    /// retry three times before giving up, failing the operation.
    /// 
    /// Since the individual user imports are independent from each other, the 
    /// ImportExternalUserOperation class is marked with the Atomic attribute, 
    /// wrapping wrapping each user import in its own transaction. Furthermore, 
    /// they are marked with the ContinueOnFailure attribute, making sure a failure 
    /// to import a single user does not halt the entire workflow.
    /// 
    /// In case there are any errors sending email notifications in the 
    /// SendWelcomeEmailOperation class, we specify a fallback behavior of sending SMS 
    /// notifications using the CompensatingOperation attribute.
    /// 
    /// This workflow illustrates a more compact style than the other examples, where
    /// all the child operations used in an operation are defined as private classes to
    /// the parent operation. The C# 6 syntax for expression-bodied members is also 
    /// used to make the code as compact as possible. This might be a better solution if 
    /// you have many, very simple operations. Having all the operations in a single file
    /// makes it easier to get an overview of the workflow. You might want to make the
    /// nested classes public instead if you want to reference them in unit tests.
    /// </summary>
    class ImportUsersWorkflow : Operation
    {
        public override IEnumerable<IOperation> GetChildOperations()
        {
            yield return Create<LoadUserDataFromConfigDefinedExternalDatabaseOperation>();
            foreach (var externalUser in GetChildOutputValues<ExternalUser>())
                yield return Create<ImportExternalUserOperation, ExternalUser>(externalUser);
        }

        private class LoadUserDataFromConfigDefinedExternalDatabaseOperation : Operation
        {
            private readonly Func<string, IExternalDatabase> _dbFactory;

            [Output]       public IExternalDatabase ExternalDb { get; private set; }
            [Output, Pipe] public IEnumerable<ExternalUser> Users { get; private set; }

            public LoadUserDataFromConfigDefinedExternalDatabaseOperation(Func<string, IExternalDatabase> dbFactory)
            {
                _dbFactory = dbFactory;
            }

            protected override void OnExecute()
            {
                ExternalDb = _dbFactory(ConfigurationManager.AppSettings["ExternalDbConnectionString"]);
            }

            public override IEnumerable<IOperation> GetChildOperations()
            {
                yield return Create<LoadUserDataFromExternalDatabaseOperation, IExternalDatabase>(ExternalDb);
            }

            /// <summary>
            /// Normally you might make this operation be at the same level as its parent operation, just running after
            /// it. However, it is added as a nested operation to illustrate how you can use the Pipe attribute to
            /// automatically pull output values out of child operations into an operation's own output property.
            /// </summary>
            [Retry]
            private class LoadUserDataFromExternalDatabaseOperation : Operation
            {
                [Input]  public IExternalDatabase ExternalDb { get; set; }
                [Output] public IEnumerable<ExternalUser> Users { get; private set; }

                protected override void OnExecute() => Users = ExternalDb.Users.Where(u => !u.IsImported).ToList();
            }
        }

        [ContinueOnFailure, Atomic]
        private class ImportExternalUserOperation : Operation
        {
            [Input, Pipe] public ExternalUser User { get; set; }
            [Input, Pipe] public IExternalDatabase ExternalDb { get; set; }

            public override IEnumerable<IOperation> GetChildOperations()
            {
                yield return Create<SanitizeExternalUserOperation>();
                yield return Create<PersistImportedUserOperation>();
                yield return Create<MarkUserAsImportedInExternalDatabaseOperation>();
                yield return Create<SendWelcomeEmailOperation>();
            }

            private class SanitizeExternalUserOperation : Operation
            {
                [Input]  public ExternalUser ExternalUser { get; set; }
                [Output] public User User { get; private set; }

                protected override void OnExecute() => User = Sanitize(ExternalUser);

                private User Sanitize(ExternalUser externalUser)
                {
                    // Do some actual work here

                    return new User();
                }
            }

            [Retry]
            private class PersistImportedUserOperation : Operation
            {
                private readonly IDatabase _db;

                [Input] public User User { get; set; }

                public PersistImportedUserOperation(IDatabase db)
                {
                    _db = db;
                }

                protected override void OnExecute() => _db.Save(User);
            }

            [Retry]
            private class MarkUserAsImportedInExternalDatabaseOperation : Operation
            {
                [Input] public ExternalUser ExternalUser { get; set; }
                [Input] public IExternalDatabase ExternalDb { get; set; }

                protected override void OnExecute()
                {
                    ExternalUser.IsImported = true;
                    ExternalDb.Save(ExternalUser);
                }
            }

            [CompensatingOperation(typeof(SendWelcomeSmsOperation)), ContinueOnFailure]
            private class SendWelcomeEmailOperation : Operation
            {
                private readonly IEmailService _emailService;

                [Input] public User User { get; set; }

                public SendWelcomeEmailOperation(IEmailService emailService)
                {
                    _emailService = emailService;
                }

                protected override void OnExecute() => _emailService.SendWelcomeEmail(User);
            }

            private class SendWelcomeSmsOperation : Operation
            {
                private readonly ISmsService _smsService;

                [Input] public User User { get; set; }

                public SendWelcomeSmsOperation(ISmsService smsService)
                {
                    _smsService = smsService;
                }

                protected override void OnExecute() => _smsService.SendWelcomeMessage(User);
            }
        }
    }
}

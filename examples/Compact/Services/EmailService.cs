using Compact.DomainClasses;
using System;

namespace Compact.Services
{
    interface IEmailService
    {
        void SendWelcomeEmail(User user);
    }

    class EmailService : IEmailService
    {
        public void SendWelcomeEmail(User user)
        {
            throw new Exception("Could not connect to smtp server.");
        }
    }
}

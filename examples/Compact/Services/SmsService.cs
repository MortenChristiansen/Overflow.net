using Compact.DomainClasses;

namespace Compact.Services
{
    interface ISmsService
    {
        void SendWelcomeMessage(User user);
    }

    class SmsService : ISmsService
    {
        public void SendWelcomeMessage(User user)
        {
        }
    }
}

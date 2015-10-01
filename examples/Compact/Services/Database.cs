using Compact.DomainClasses;

namespace Compact.Services
{
    interface IDatabase
    {
        void Save(User user);
    }

    class Database : IDatabase
    {
        public void Save(User user)
        {
        }
    }
}

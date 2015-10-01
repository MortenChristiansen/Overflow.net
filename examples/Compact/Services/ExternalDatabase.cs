using Compact.DomainClasses;
using System.Linq;

namespace Compact.Services
{
    interface IExternalDatabase
    {
        IQueryable<ExternalUser> Users { get; }

        void Save(ExternalUser user);
    }

    class ExternalDatabase : IExternalDatabase
    {
        public IQueryable<ExternalUser> Users { get; } = new[] { new ExternalUser(), new ExternalUser() }.AsQueryable();
        public void Save(ExternalUser user)
        {
        }
    }
}

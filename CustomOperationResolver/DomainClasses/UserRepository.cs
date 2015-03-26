using System.Collections.Generic;

namespace CustomOperationResolver.DomainClasses
{
    class UserRepository
    {
        public IList<User> GetUsersWithoutLocation()
        {
            return new[]
            {
                new User { Name = "Peter Sellers", LastIpAddress = "43.52.54.11" },
                new User { Name = "Jane Fonda", LastIpAddress = "123.42.122.5" }
            };
        }

        public void SaveChanges(User user)
        {
            
        }
    }
}

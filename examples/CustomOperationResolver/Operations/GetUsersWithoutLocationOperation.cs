using System.Collections.Generic;
using CustomOperationResolver.DomainClasses;
using Overflow;

namespace CustomOperationResolver.Operations
{
    class GetUsersWithoutLocationOperation : Operation
    {
        private readonly UserRepository _repository;

        [Output]
        public IEnumerable<User> Users { get; private set; }

        public GetUsersWithoutLocationOperation(UserRepository repository)
        {
            _repository = repository;
        }

        protected override void OnExecute()
        {
            Users = _repository.GetUsersWithoutLocation();
        }
    }
}

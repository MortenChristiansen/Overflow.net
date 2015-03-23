using System;
using System.Collections.Generic;
using CustomOperationResolver.DomainClasses;
using Overflow;

namespace CustomOperationResolver.Operations
{
    class GetUsersWithoutLocationOperation : Operation, IOutputOperation<IEnumerable<User>>
    {
        private readonly UserRepository _repository;
        private Action<IEnumerable<User>> _onReceiveOutput;

        public GetUsersWithoutLocationOperation(UserRepository repository)
        {
            _repository = repository;
        }

        public void Output(Action<IEnumerable<User>> onReceiveOutput)
        {
            _onReceiveOutput = onReceiveOutput;
        }

        protected override void OnExecute()
        {
            var users = _repository.GetUsersWithoutLocation();
            _onReceiveOutput(users);
        }
    }
}

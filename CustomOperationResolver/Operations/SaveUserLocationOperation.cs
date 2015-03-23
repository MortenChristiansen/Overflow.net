using CustomOperationResolver.DomainClasses;
using Overflow;

namespace CustomOperationResolver.Operations
{
    class SaveUserLocationOperation : Operation, IInputOperation<User>
    {
        private readonly UserRepository _repository;
        private User _user;

        public SaveUserLocationOperation(UserRepository repository)
        {
            _repository = repository;
        }

        public void Input(User input)
        {
            _user = input;
        }

        protected override void OnExecute()
        {
            _repository.SaveChanges(_user);
        }
    }
}

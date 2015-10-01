using CustomOperationResolver.DomainClasses;
using Overflow;

namespace CustomOperationResolver.Operations
{
    class SaveUserLocationOperation : Operation
    {
        private readonly UserRepository _repository;

        [Input]
        public User User { get; set; }

        public SaveUserLocationOperation(UserRepository repository)
        {
            _repository = repository;
        }

        protected override void OnExecute()
        {
            _repository.SaveChanges(User);
        }
    }
}

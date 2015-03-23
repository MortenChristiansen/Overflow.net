using CustomOperationResolver.DomainClasses;
using Overflow;

namespace CustomOperationResolver.Operations
{
    [Retry]
    class UpdateUserLocationOperation : Operation, IInputOperation<User>
    {
        private readonly UserLocationLookupService _service;
        private User _user;

        public UpdateUserLocationOperation(UserLocationLookupService service)
        {
            _service = service;
        }

        public void Input(User input)
        {
            _user = input;
        }

        protected override void OnExecute()
        {
            _user.Location = _service.GetLocation(_user.LastIpAddress);
        }
    }
}

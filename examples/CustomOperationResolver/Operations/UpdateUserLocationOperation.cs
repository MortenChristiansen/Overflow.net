using CustomOperationResolver.DomainClasses;
using Overflow;

namespace CustomOperationResolver.Operations
{
    [Retry]
    class UpdateUserLocationOperation : Operation
    {
        private readonly UserLocationLookupService _service;

        [Input]
        public User User { get; set; }

        public UpdateUserLocationOperation(UserLocationLookupService service)
        {
            _service = service;
        }

        protected override void OnExecute()
        {
            User.Location = _service.GetLocation(User.LastIpAddress);
        }
    }
}

using AutoApi.Mediator;
using AutoApi.Rest.Shared.Requests;

namespace AutoApi.Rest.RequestManagement;

public class UpdateCommand<TCommand, TResponse> :
    CommandRequest<TCommand>, IRequest<TResponse>
    where TCommand : IModifyRequest
{
    public UpdateCommand(Guid id, TCommand request) : base(request) => Id = id;
}

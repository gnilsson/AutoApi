using AutoApi.Mediator;
using AutoApi.Rest.Shared.Requests;

namespace AutoApi.Rest.RequestManagement;

public class CreateCommand<TCommand, TResponse> :
    CommandRequest<TCommand>, IRequest<TResponse>
    where TCommand : ICommandRequest
{
    public CreateCommand(TCommand request) : base(request) { }
}

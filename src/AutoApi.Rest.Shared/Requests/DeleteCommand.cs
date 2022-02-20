using AutoApi.Mediator;
using AutoApi.Rest.Shared.Responses;

namespace AutoApi.Rest.Shared.Requests;

public class DeleteCommand : IRequest<DeleteResponse>
{
    public DeleteCommand(Guid entityId) => Id = entityId;

    public Guid Id { get; }
}

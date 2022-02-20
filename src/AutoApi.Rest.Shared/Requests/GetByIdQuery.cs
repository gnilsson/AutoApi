using AutoApi.Mediator;
using GN.Toolkit;

namespace AutoApi.Rest.Shared.Requests;

public class GetByIdQuery<TResponse> : IRequest<TResponse>
{
    public GetByIdQuery(Identifier id) => Id = id;
    public Identifier Id { get; }
}

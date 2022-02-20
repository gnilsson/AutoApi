using AutoApi.Rest.Shared.Responses;
using GN.Toolkit;

namespace AutoApi.Sample.Shared;

public interface IBlogResponseSimplified : ISimplified
{
    public Identifier Id { get; init; }
    public string Title { get; init; }
}

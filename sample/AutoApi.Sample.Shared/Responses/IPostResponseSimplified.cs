using AutoApi.Rest.Shared.Responses;
using GN.Toolkit;

namespace AutoApi.Sample.Shared;

public interface IPostResponseSimplified : ISimplified
{
    public string? Id { get; init; }
    public string? Title { get; init; }
}

using AutoApi.Rest.Shared.Responses;
using GN.Toolkit;

namespace AutoApi.Sample.Shared;

public class PostResponse : EntityResponse, IPostResponseSimplified
{
    public string? Title { get; init; }
    public string? Content { get; init; }
    public Identifier BlogId { get; init; } = default!;
}

using AutoApi.Rest.Shared.Responses;
using GN.Toolkit;

namespace AutoApi.Sample.Shared;

public class PostResponse : EntityResponse, IPostResponseSimplified
{
    public string? Title { get; init; }
    public string? Content { get; init; }
    public string? BlogId { get; init; }
}

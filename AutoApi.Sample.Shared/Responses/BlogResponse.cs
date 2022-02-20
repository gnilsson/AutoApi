using AutoApi.Rest.Shared.Responses;

namespace AutoApi.Sample.Shared;

public class BlogResponse : EntityResponse, IBlogResponseSimplified
{
    public string Title { get; init; } = default!;
    public IEnumerable<IPostResponseSimplified>? Posts { get; init; }
    public string? BlogCategory { get; init; }
    public IEnumerable<IAuthorResponseSimplified>? Authors { get; init; }
}

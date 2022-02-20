using AutoApi.Rest.Shared.Responses;

namespace AutoApi.Sample.Shared;

public class AuthorResponse : EntityResponse, IAuthorResponseSimplified
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Profession { get; set; }
    public IEnumerable<IBlogResponseSimplified>? Blogs { get; set; }
}

using AutoApi.Rest.Shared.Responses;

namespace AutoApi.Sample.Shared;

public interface IAuthorResponseSimplified : ISimplified
{
    public string? FirstName { get; set; }
}

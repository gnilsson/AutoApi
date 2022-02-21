using GN.Toolkit;

namespace AutoApi.Rest.Shared.Responses;

public interface IEntityResponse
{
    public string? Id { get; init; }
    public string? CreatedDate { get; init; }
    public string? UpdatedDate { get; init; }
}

public abstract class EntityResponse : IEntityResponse
{
    public string? Id { get; init; }
    public string? CreatedDate { get; init; }
    public string? UpdatedDate { get; init; }
}

namespace AutoApi.Rest.Shared.Requests;

public interface IGetRequest : IPaginateable
{
    public string? OrderBy { get; init; }
    public string? CreatedDate { get; init; }
    public string? UpdatedDate { get; init; }
    public string? Expand { get; init; }
}

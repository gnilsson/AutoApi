namespace AutoApi.Rest.Shared.Requests;

public abstract class GetRequest : IGetRequest
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? OrderBy { get; init; }
    public string? CreatedDate { get; init; }
    public string? UpdatedDate { get; init; }
    public string? Expand { get; init; }
}

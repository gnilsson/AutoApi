namespace AutoApi.Rest.Shared.Responses;

public record PaginateableResponse<TResponse>
{
    public IEnumerable<TResponse>? Data { get; init; }

    public int? PageNumber { get; init; }

    public int? PageSize { get; init; }

    public string? NextPage { get; init; }

    public string? PreviousPage { get; init; }

    public int? Total { get; init; }

    public IEnumerable<string>? Errors { get; init; }
}

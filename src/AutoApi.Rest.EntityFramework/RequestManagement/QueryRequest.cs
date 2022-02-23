using AutoApi.QueryRequestDefinition.Parameters;
using AutoApi.Rest.Shared.Requests;
using AutoApi.Rest.Shared.Requests.Parameters;

namespace AutoApi.Rest.EntityFramework.RequestManagement;

public class QueryRequest
{
    public QueryRequest(IGetRequest query) => Query = query;

    public IGetRequest Query { get; } = default!;
    public ICollection<IParameter>? Parameters { get; set; }
    public PaginationQuery PaginationQuery { get; set; } = default!;
    public string? RequestRoute { get; set; }
    public OrderByParameter? OrderByParameter { get; set; }
    public string[]? ExpandMembers { get; set; }
    public ICollection<string>? Errors { get; set; }
    public Guid[]? ForeignIds { get; init; }
}

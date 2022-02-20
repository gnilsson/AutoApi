namespace AutoApi.QueryRequestDefinition;

public class QueryResult<TResponse> : IQueryResult<TResponse>
{
    public int? Total { get; }
    public ICollection<TResponse> Items { get; }

    public QueryResult(int? total, ICollection<TResponse> items)
    {
        Total = total;
        Items = items;
    }
}

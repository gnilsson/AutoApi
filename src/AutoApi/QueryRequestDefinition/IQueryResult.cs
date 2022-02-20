using System.Collections.Generic;

namespace AutoApi.QueryRequestDefinition;

public interface IQueryResult<TResponse>
{
    public int? Total { get; }
    public ICollection<TResponse> Items { get; }
}

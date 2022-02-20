using AutoApi.QueryRequestDefinition;

namespace AutoApi.Rest.Configuration;

public class QueryConfiguration<T> : IQueryConfiguration
{
    public string[]? ExpandMembers { get; init; }
    public Type ApiQueryType { get; init; } = default!;
}

namespace AutoApi.QueryRequestDefinition;

public interface IQueryConfiguration
{
    public string[]? ExpandMembers { get; init; }

    public Type ApiQueryType { get; init; }
}

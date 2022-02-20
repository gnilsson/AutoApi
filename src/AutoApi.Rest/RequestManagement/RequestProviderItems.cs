using AutoApi.QueryRequestDefinition;
using AutoApi.Rest.EntityFramework.RequestManagement;

namespace AutoApi.Rest.RequestManagement;

public class RequestProviderItems
{
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, QueryParameterShell>> Parameters { get; init; } = default!;

    public PaginationSettings PaginationSettings { get; init; } = default!;
}

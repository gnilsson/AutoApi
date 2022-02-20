using AutoApi.Domain;
using AutoApi.Rest.Shared.Requests.Parameters;

namespace AutoApi.QueryRequestDefinitions.Parameters;

public class CreatedDateParameter : DateParameter
{
    public CreatedDateParameter(string value, string[]? navigationNodes)
        : base(value, navigationNodes ?? new[] { nameof(IEntity.CreatedDate) })
    { }
}

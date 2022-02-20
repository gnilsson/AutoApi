using AutoApi.Domain;
using AutoApi.Rest.Shared.Requests.Parameters;
using System.Data;

namespace AutoApi.QueryRequestDefinition.Parameters;

public class UpdatedDateParameter : DateParameter
{
    public UpdatedDateParameter(string value, string[]? navigationNodes)
        : base(value, navigationNodes ?? new[] { nameof(IEntity.UpdatedDate) })
    { }
}

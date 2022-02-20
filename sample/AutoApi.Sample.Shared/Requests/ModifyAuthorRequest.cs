using AutoApi.Rest.Shared.Attributes;
using AutoApi.Rest.Shared.Requests;
using AutoApi.Sample.Shared.Entities;
using AutoApi.Sample.Shared.Enums;

namespace AutoApi.Sample.Shared.Requests;

public class ModifyAuthorRequest : IModifyRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public ProfessionCategory? Profession { get; set; }
    [IdCollection(typeof(Blog))]
    public ICollection<Guid>? BlogIds { get; set; }
}

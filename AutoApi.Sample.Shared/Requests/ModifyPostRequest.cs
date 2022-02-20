using System.ComponentModel.DataAnnotations;
using AutoApi.Rest.Shared.Requests;

namespace AutoApi.Sample.Shared.Requests;

public class ModifyPostRequest : IModifyRequest
{
    [Required]
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid BlogId { get; set; }
}

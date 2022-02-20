using System.ComponentModel.DataAnnotations;
using AutoApi.Rest.Shared.Attributes;
using AutoApi.Rest.Shared.Requests;
using AutoApi.Sample.Shared.Entities;
using AutoApi.Sample.Shared.Enums;

namespace AutoApi.Sample.Shared.Requests;

public class ModifyBlogRequest : IModifyRequest
{
    [Required]
    public string Title { get; set; } = default!;
    public BlogCategory? BlogCategory { get; set; }
    [IdCollection(typeof(Author))]
    public ICollection<Guid>? AuthorIds { get; set; }
}

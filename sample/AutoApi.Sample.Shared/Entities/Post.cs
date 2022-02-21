using System.ComponentModel.DataAnnotations;
using AutoApi.Domain;
using GN.Toolkit;

namespace AutoApi.Sample.Shared.Entities;

public class Post : IEntity
{
    public Post() { }

    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    [Required]
    public Blog Blog { get; set; } = default!;
    public Identifier BlogId { get; set; } = default!;
    public Identifier Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}

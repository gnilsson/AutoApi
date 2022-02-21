using AutoApi.Domain;
using AutoApi.Sample.Shared.Enums;
using GN.Toolkit;

namespace AutoApi.Sample.Shared.Entities;

public class Blog : IEntity
{
    public Blog()
    {
        this.Posts = new HashSet<Post>();
        this.Authors = new HashSet<Author>();
    }

    public string? Title { get; set; }
    public BlogCategory BlogCategory { get; set; }
    public ICollection<Post> Posts { get; set; }
    public ICollection<Author> Authors { get; set; }
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}

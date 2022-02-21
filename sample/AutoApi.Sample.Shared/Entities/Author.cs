using AutoApi.Domain;
using AutoApi.Sample.Shared.Enums;
using GN.Toolkit;

namespace AutoApi.Sample.Shared.Entities;

public class Author : IEntity
{
    public Author()
    {
        this.Blogs = new HashSet<Blog>();
    }

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public ProfessionCategory Profession { get; set; }
    public ICollection<Blog> Blogs { get; set; }
    public Identifier Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}

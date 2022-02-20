using GN.Toolkit;

namespace AutoApi.Domain;

public interface IEntity
{
    public Identifier Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}

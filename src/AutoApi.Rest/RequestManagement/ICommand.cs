using GN.Toolkit;

namespace AutoApi.Rest.RequestManagement;

public interface ICommand
{
    public string[]? IgnoredProperties { get; set; }
    public Dictionary<string, object> RequestPropertyValues { get; }
    public Identifier Id { get; }
    public string? IncludeNavigation { get; set; }
    public Dictionary<string, Type> RequestForeignEntities { get; }
}

using AutoApi.Rest.Shared.Requests;
using GN.Toolkit;

namespace AutoApi.Rest.RequestManagement;

public abstract class CommandRequest<TCommand> :
    ICommand,
    ICommandRequest<TCommand>
    where TCommand : IModifyRequest
{
    public string[]? IgnoredProperties { get; set; }
    public Dictionary<string, object> RequestPropertyValues { get; } = new();
    public Identifier Id { get; internal set; }
    public TCommand Command { get; }
    public string? IncludeNavigation { get; set; }
    public Dictionary<string, Type> RequestForeignEntities { get; } = new();

    public CommandRequest(TCommand request)
    {
        Command = request;
    }
}

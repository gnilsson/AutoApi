using AutoApi.Rest.Shared.Requests;

namespace AutoApi.Rest.RequestManagement;

public interface ICommandRequest<out TCommand> where TCommand : ICommandRequest
{
    public TCommand Command { get; }
}

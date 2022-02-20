using AutoApi.Rest.Shared.Requests;

namespace AutoApi.Rest.RequestManagement;

public interface ICommandRequest<out TCommand> where TCommand : IModifyRequest
{
    public TCommand Command { get; }
}

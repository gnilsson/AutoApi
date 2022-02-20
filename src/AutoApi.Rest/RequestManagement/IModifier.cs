using AutoApi.Domain;

namespace AutoApi.Rest.RequestManagement;

public interface IModifier<TEntity, TCommand> where TEntity : IEntity where TCommand : ICommand
{
    public Func<TCommand, TEntity> Create { get; }
    public Action<TEntity, TCommand> Update { get; }
}

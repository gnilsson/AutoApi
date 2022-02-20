using AutoApi.Domain;

namespace AutoApi.Rest.RequestManagement;

public class Modifier<TEntity, TCommand> :
             IModifier<TEntity, TCommand>
             where TEntity : IEntity
             where TCommand : ICommand
{
    public Modifier()
    {
        Create = ModifierOperations<TEntity, TCommand>.GetCreateOperation();
        Update = ModifierOperations<TEntity, TCommand>.GetUpdateOperation();
    }

    public Func<TCommand, TEntity> Create { get; }
    public Action<TEntity, TCommand> Update { get; }
}

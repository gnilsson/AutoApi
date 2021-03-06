namespace AutoApi.EntityFramework.Repository;

public abstract class RepositoryConcept<TContext>
{
    internal TContext DbContext { get; }
    public RepositoryConcept(TContext context) => DbContext = context;
}

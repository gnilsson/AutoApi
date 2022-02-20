using System.Collections.Concurrent;
using AutoApi.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AutoApi.EntityFramework.Repository;

public class RepositoryWrapper<TContext> :
             IRepositoryWrapper
             where TContext : DbContext
{
    private readonly TContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ConcurrentDictionary<string, object> _repoCache;
    private readonly IReadOnlyDictionary<string, RepositoryShell> _repositoryShellContainer;

    private IGeneralRepository? _general;

    public RepositoryWrapper(
        TContext dbContext,
        IMapper mapper,
        IReadOnlyDictionary<string, RepositoryShell> repositoryShellContainer) =>
        (_dbContext, _mapper, _repoCache, _repositoryShellContainer) =
        (dbContext, mapper, new ConcurrentDictionary<string, object>(), repositoryShellContainer);

    public IGeneralRepository General => _general ??= new GeneralRepository<TContext>(_dbContext);

    public IRepository<TEntity, TResponse> Get<TEntity, TResponse>() where TEntity : IEntity
        => (IRepository<TEntity, TResponse>)(
            _repoCache.ContainsKey(typeof(TEntity).Name) ?
            _repoCache[typeof(TEntity).Name] :
            _repoCache.GetOrAdd(
            typeof(TEntity).Name,
            Fetch<TEntity, TResponse>()));

    private IRepository<TEntity, TResponse> Fetch<TEntity, TResponse>()
    {
        var shell = _repositoryShellContainer.FirstOrDefault(x => x.Key == typeof(TEntity).Name).Value;

        return (shell.Constructor(_dbContext, _mapper, shell.QueryConfiguration) as IRepository<TEntity, TResponse>)!;
    }

    public async Task SaveAsync() => await _dbContext.SaveChangesAsync();
}

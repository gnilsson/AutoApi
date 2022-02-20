using System.Linq.Expressions;
using AutoApi.QueryRequestDefinition;
using AutoApi.Rest.EntityFramework.RequestManagement;

namespace AutoApi.EntityFramework.Repository;

public interface IRepository<TEntity, TResponse>
{
    ValueTask<TEntity?> FindAsync(Guid entityId, CancellationToken token);
    Task<List<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token);
    Task<TResponse?> GetOneAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token);
    Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> predicate, string includeNavigation, CancellationToken token);
    void Delete(TEntity entity);
    Task CreateAsync(TEntity entity, CancellationToken token);
    Task<IQueryResult<TResponse>> GetQueriedResultAsync(QueryRequest queryRequest, CancellationToken token);
}

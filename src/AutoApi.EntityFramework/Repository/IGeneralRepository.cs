using System.Linq.Expressions;
using AutoApi.Domain;
using GN.Toolkit;

namespace AutoApi.EntityFramework.Repository;

public interface IGeneralRepository
{
    Task<TEntity?> GetFirstByConditionAsync<TEntity>(
                        Expression<Func<TEntity, bool>> predicate,
                        CancellationToken token)
                        where TEntity : class;
    IQueryable<TEntity> FindByCondition<TEntity>(
                        Expression<Func<TEntity, bool>> predicate)
                        where TEntity : class;
    ValueTask<TEntity> FindAsync<TEntity>(Identifier entityId,
                        CancellationToken token)
                        where TEntity : class, IEntity;
    Task<List<TEntity>> GetManyAsync<TEntity>(
                        IEnumerable<Guid> entityIds,
                        CancellationToken token)
                        where TEntity : class, IEntity;
    Task CreateAsync<TEntity>(TEntity entity,
                        CancellationToken token)
                        where TEntity : class;
    Task CreateManyAsync<TEntity>(IEnumerable<TEntity> entities,
                        CancellationToken token)
                        where TEntity : class;
    void Delete<TEntity>(TEntity entity)
                        where TEntity : class;
}

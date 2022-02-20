using System.Linq.Expressions;
using AutoApi.Domain;
using AutoApi.QueryRequestDefinition;
using AutoApi.QueryRequestDefinition.Expressions;
using AutoApi.Rest.EntityFramework.RequestManagement;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AutoApi.EntityFramework.Repository;

public class Repository<TEntity, TResponse, TContext> :
             RepositoryConcept<TContext>,
             IRepository<TEntity, TResponse>
             where TEntity : class, IEntity
             where TContext : DbContext
{
    private readonly IMapper _mapper;
    private readonly string[] _expandMembers;
    private readonly IQueryInstructions<TEntity>.QueryDelegate _querier;

    public Repository(TContext context, IMapper mapper, QueryManager<TEntity> queryManager) : base(context)
    {
        _mapper = mapper;
        _expandMembers = queryManager.ExpandMembers;
        _querier = queryManager.Querier;
    }

    private DbSet<TEntity> Set() => DbContext.Set<TEntity>();

    private IQueryable<TEntity> SetQuery() => DbContext.Set<TEntity>();

    public async Task<IQueryResult<TResponse>> GetQueriedResultAsync(QueryRequest queryRequest, CancellationToken token)
    {
        var query = SetQuery().AsNoTracking();

        if (queryRequest.ForeignIds is not null)
        {
            query = query.Where(x => queryRequest.ForeignIds.Contains(x.Id));
        }

        if (queryRequest.Parameters is not null)
        {
            query = query.Where(_querier(queryRequest.Parameters));
        }

        if (queryRequest.OrderByParameter is not null)
        {
            query = query.OrderBy(queryRequest.OrderByParameter);
        }

        return new QueryResult<TResponse>(
            await query.CountAsync(token),
            await query.ApplyPaging(queryRequest.PaginationQuery)
            .ProjectTo<TResponse, TEntity>(_mapper.ConfigurationProvider, queryRequest.ExpandMembers ?? _expandMembers)
            .ToListAsync(token));
    }

    public async Task<TResponse?> GetOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken token)
      => await SetQuery()
            .AsNoTracking()
            .Where(predicate)
            .ProjectTo<TResponse, TEntity>(_mapper.ConfigurationProvider, _expandMembers)
            .FirstOrDefaultAsync(token);

    public async ValueTask<TEntity?> FindAsync(
        Guid entityId, CancellationToken token)
        => await Set().FindAsync(new object[] { entityId }, token);

    public async Task<List<TEntity>> GetManyAsync(
        Expression<Func<TEntity, bool>> predicate, CancellationToken token)
       => await FindByCondition(predicate).ToListAsync(token);

    private IQueryable<TEntity> FindByCondition(
         Expression<Func<TEntity, bool>> predicate)
       => SetQuery().Where(predicate);

    public async Task<TEntity?> GetOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        string includeNavigation,
        CancellationToken token)
      => await SetQuery()
            .Where(predicate)
            .Include(includeNavigation)
            .FirstOrDefaultAsync(token);

    public void Delete(TEntity entity)
        => Set().Remove(entity);

    public async Task CreateAsync(
        TEntity entity, CancellationToken token)
       => await Set().AddAsync(entity, token);
}

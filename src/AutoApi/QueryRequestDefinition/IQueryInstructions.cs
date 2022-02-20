using System.Linq.Expressions;
using AutoApi.Domain;
using AutoApi.Rest.Shared.Requests.Parameters;

namespace AutoApi.QueryRequestDefinition;

public interface IQueryInstructions<TEntity> where TEntity : class, IEntity
{
    public QueryDelegate Query { get; }

    public delegate Expression<Func<TEntity, bool>> QueryDelegate(IEnumerable<IParameter> parameters);
}

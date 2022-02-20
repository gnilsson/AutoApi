using AutoApi.Domain;
using AutoApi.QueryRequestDefinition;

namespace AutoApi.Rest.EntityFramework.RequestManagement;

public class QueryManager<TEntity> where TEntity : class, IEntity
{
    public IQueryInstructions<TEntity>.QueryDelegate Querier { get; }

    public string[] ExpandMembers { get; } = default!;

    public QueryManager(IQueryConfiguration config)
    {
        //var inst = ExpressionUtility.CreateEmptyConstructor(config.ApiQueryType) as IQueryInstructions<TEntity>;
        //Querier = inst!.Query;
        var instructions = new QueryInstructions<TEntity>();
        Querier = instructions.Query;
        ExpandMembers = config.ExpandMembers?.ToArray()!;
    }
}

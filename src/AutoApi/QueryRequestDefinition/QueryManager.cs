using AutoApi.Domain;
using AutoApi.Utility;

namespace AutoApi.QueryRequestDefinition;

//public class QueryManager<TEntity> where TEntity : class, IEntity
//{
//    public IQueryInstructions<TEntity>.QueryDelegate Querier { get; }

//    public string[] ExpandMembers { get; } = default!;

//    public QueryManager(IQueryConfiguration config)
//    {
//        var inst = ExpressionUtility.CreateEmptyConstructor(config.ApiQueryType) as IQueryInstructions<TEntity>;
//        //var instructions = new QueryInstructions<TEntity>();
//        Querier = inst!.Query;
//        ExpandMembers = config.ExpandMembers?.ToArray()!;
//    }
//}

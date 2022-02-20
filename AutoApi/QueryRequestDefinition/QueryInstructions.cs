


//using System.Linq.Expressions;
//using AutoApi.Defined;
//using AutoApi.Defined.Descriptive;
//using AutoApi.Domain;
//using AutoApi.QueryRequestDefinition.Expressions;
//using AutoApi.Rest.Shared.Requests.Parameters;

//namespace AutoApi.QueryRequestDefinition;

//public class QueryInstructions<TEntity> where TEntity : class, IEntity
//{
//    private readonly ConfigurationDefinition.Entity[] _foreignEntityDefinitions;

//    public QueryDelegate Query { get; }

//    public delegate Expression<Func<TEntity, bool>> QueryDelegate(IEnumerable<IParameter> parameters);

//    public QueryInstructions(ConfigurationDefinition[] foreignEntityDefinitions)
//    {
//        Query = QueryHandle;
//        _foreignEntityDefinitions = foreignEntityDefinitions;
//    }

//    private Expression<Func<TEntity, bool>> QueryHandle(IEnumerable<IParameter> parameters)
//    {
//        Expression<Func<TEntity, bool>> predicate = p => true;

//        //if (nestedQueryTarget is not null)
//        //{
//        //    //var dirtyEntityName = nestedQueryTarget.Type.Name.Split("Response")[0] + "s";
//        //    var foreign = _foreignEntityDefinitions.FirstOrDefault(x => x.ResponseType == nestedQueryTarget.Type);
//        //    var baseProperty = Expression.Parameter(typeof(TEntity));
//        //    var field = Expression.PropertyOrField(baseProperty, $"{foreign.EntityType.Name}s");
//        //    var hm = Expression.Parameter(foreign.EntityType);
//        //    var idField = Expression.PropertyOrField(hm, "Id");

//        //    var call = Expression.Call(typeof(IEnumerable<Guid>), "Select", null, field, idField); // new[] { field.Type, typeof(Guid) }
//        //    var cons = Expression.Constant(nestedQueryTarget.Id);

//        //    var contains = typeof(Guid).GetMethod(nameof(Method.Contains), new[] { typeof(Guid) });
//        //    var cont = Expression.Call(call, contains, cons);

//        //    var ah = Expression.Lambda<Func<TEntity, bool>>(cont, baseProperty);
//        //    return ah;

//        //  //  var callSelect = Expression.Call(null, )
//        //    //var access = Expression.ArrayAccess(field);

//        //}

//        foreach (var parameter in parameters)
//        {
//            predicate = QueryExpressionUtility.AndAlso(predicate, Filter(parameter));
//        }

//        return predicate;
//    }

//    private Expression<Func<TEntity, bool>> Filter(IParameter parameter)
//    {
//        var baseProperty = Expression.Parameter(typeof(TEntity));

//        var queries = InvokeApplicableCallMethod(GetChildrenMembers(baseProperty, parameter), parameter);

//        return Expression.Lambda<Func<TEntity, bool>>(queries, baseProperty);
//    }

//    private MemberExpression[] GetChildrenMembers(Expression baseProperty, IParameter parameter)
//    {
//        var parent = parameter.NestingNavigationProperties == null
//            ? baseProperty
//            : GetProperty(baseProperty, parameter.NestingNavigationProperties);

//        return parameter.LocalNavigationProperties!
//            .Select(child => Expression.PropertyOrField(parent, child))
//            .ToArray();
//    }

//    private Expression GetProperty(Expression parameter, string[] nodes, int iterator = 0)
//    {
//        var next = Expression.PropertyOrField(parameter, nodes[iterator]);

//        if (iterator < nodes.Length - 1)
//        {
//            GetProperty(next, nodes, iterator++);
//        }

//        return next;
//    }

//    private Expression InvokeApplicableCallMethod(MemberExpression[] members, IParameter parameter)
//    {
//        if (parameter.Method == QueryMethod.Equal)
//        {
//            return Expression.Equal(members[0], Expression.Constant(parameter.Value));
//        }

//        return (Expression)MethodFactory.QueryMethodContainer
//            .FirstOrDefault(x => x.Key == parameter.Method).Value
//            .Invoke(this, new Expression[]
//            {
//                Expression.NewArrayInit(parameter.Value!.GetType(), members),
//                Expression.Constant(parameter.Value),
//                default!,
//            })!;
//    }
//}

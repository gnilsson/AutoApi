using System.Linq.Expressions;
using AutoApi.Defined;
using AutoApi.Defined.Descriptive;
using AutoApi.Domain;
using AutoApi.QueryRequestDefinition;
using AutoApi.QueryRequestDefinition.Expressions;
using AutoApi.Rest.Shared.Requests.Parameters;

namespace AutoApi.Rest.EntityFramework.RequestManagement;

public class QueryInstructions<TEntity> : IQueryInstructions<TEntity> where TEntity : class, IEntity
{
    public IQueryInstructions<TEntity>.QueryDelegate Query { get; }


    public QueryInstructions()
    {
        Query = QueryHandle;
    }

    private Expression<Func<TEntity, bool>> QueryHandle(IEnumerable<IParameter> parameters)
    {
        Expression<Func<TEntity, bool>> predicate = p => true;

        foreach (var parameter in parameters)
        {
            predicate = QueryExpressionUtility.AndAlso(predicate, Filter(parameter));
        }

        return predicate;
    }

    private Expression<Func<TEntity, bool>> Filter(IParameter parameter)
    {
        var baseProperty = Expression.Parameter(typeof(TEntity));

        var queries = InvokeApplicableCallMethod(GetChildrenMembers(baseProperty, parameter), parameter);

        return Expression.Lambda<Func<TEntity, bool>>(queries, baseProperty);
    }

    private MemberExpression[] GetChildrenMembers(Expression baseProperty, IParameter parameter)
    {
        var parent = parameter.NestingNavigationProperties == null
            ? baseProperty
            : GetProperty(baseProperty, parameter.NestingNavigationProperties);

        return parameter.LocalNavigationProperties!
            .Select(child => Expression.PropertyOrField(parent, child))
            .ToArray();
    }

    private Expression GetProperty(Expression parameter, string[] nodes, int iterator = 0)
    {
        var next = Expression.PropertyOrField(parameter, nodes[iterator]);

        if (iterator < nodes.Length - 1)
        {
            GetProperty(next, nodes, iterator++);
        }

        return next;
    }

    private Expression InvokeApplicableCallMethod(MemberExpression[] members, IParameter parameter)
    {
        if (parameter.Method == QueryMethod.Equal)
        {
            return Expression.Equal(members[0], Expression.Constant(parameter.Value));
        }

        return (Expression)MethodFactory.QueryMethodContainer
            .FirstOrDefault(x => x.Key == parameter.Method).Value
            .Invoke(this, new Expression[]
            {
                Expression.NewArrayInit(parameter.Value!.GetType(), members),
                Expression.Constant(parameter.Value),
                default!,
            })!;
    }
}

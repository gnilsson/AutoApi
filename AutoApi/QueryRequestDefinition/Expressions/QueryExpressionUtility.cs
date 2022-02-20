using System.ComponentModel;
using System.Linq.Expressions;
using AutoApi.Defined;
using AutoApi.Defined.Descriptive;
using AutoApi.Domain;
using AutoApi.QueryRequestDefinition.Parameters;

namespace AutoApi.QueryRequestDefinition.Expressions;

public static class QueryExpressionUtility
{
    public static IOrderedQueryable<TEntity> OrderBy<TEntity>(
         this IQueryable<TEntity> source,
         OrderByParameter orderParameter)
    {
        var type = typeof(TEntity);
        var parameter = Expression.Parameter(type, "p");
        var property = Expression.Property(parameter, orderParameter.Node);

        var propertyAccess = Expression.MakeMemberAccess(parameter, property.Member);
        var orderByExpr = Expression.Lambda(propertyAccess, parameter);
        var methodName = orderParameter.SortDirection == ListSortDirection.Ascending ?
            Method.OrderBy :
            Method.OrderByDescending;

        var callExpr = Expression.Call(
            typeof(Queryable), methodName,
            new Type[] { type, property.Type },
            source.Expression, Expression.Quote(orderByExpr));

        return (source.Provider.CreateQuery<TEntity>(callExpr) as IOrderedQueryable<TEntity>)!;
    }

    public static Expression CallStringContains(
        NewArrayExpression members,
        ConstantExpression value,
        int? nIterator)
    {
        var iterator = nIterator ?? 0;
        var containsLeft = Expression.Call(members.Expressions[iterator], MethodFactory.Contains, value);

        if (iterator >= members.Expressions.Count - 1) return containsLeft;

        iterator++;
        return Expression.Or(containsLeft, CallStringContains(members, value, iterator));
    }

    public static Expression CallContains(
        NewArrayExpression members,
        ConstantExpression value,
        int? nIterator)
    {
        var iterator = nIterator ?? 0;
        var containsLeft = Expression.Call(members.Expressions[iterator], MethodFactory.Contains, value);

        if (iterator >= members.Expressions.Count - 1) return containsLeft;

        iterator++;
        return Expression.Or(containsLeft, CallContains(members, value, iterator));
    }


    public static Expression CallDateTimeCompare(
            NewArrayExpression members,
            ConstantExpression value,
            object _)
    {
        return Expression.LessThan(
            Expression.Constant(0),
            Expression.Call(
                members.Expressions[0], MethodFactory.CompareTo, value));
    }

    public static Expression<Func<TEntity, bool>> AndAlso<TEntity>(
            Expression<Func<TEntity, bool>> expr1,
            Expression<Func<TEntity, bool>> expr2)
            where TEntity : class, IEntity
    {
        ParameterExpression param = expr1.Parameters[0];
        if (ReferenceEquals(param, expr2.Parameters[0]))
        {
            return Expression.Lambda<Func<TEntity, bool>>(
                Expression.AndAlso(expr1.Body, expr2.Body), param);
        }
        return Expression.Lambda<Func<TEntity, bool>>(
            Expression.AndAlso(
                expr1.Body,
                Expression.Invoke(expr2, param)), param);
    }
}

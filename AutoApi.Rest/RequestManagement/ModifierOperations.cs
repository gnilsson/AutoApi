using System.Linq.Expressions;
using System.Reflection;
using AutoApi.Defined.Descriptive;
using AutoApi.Domain;

namespace AutoApi.Rest.RequestManagement;

public static class ModifierOperations<TEntity, TCommand> where TCommand : ICommand
{
    private static readonly MemberInfo _idMember = typeof(TEntity).GetMember(nameof(IEntity.Id))[0];
    private static readonly MemberInfo _createdMember = typeof(TEntity).GetMember(nameof(IEntity.CreatedDate))[0];
    private static readonly MemberInfo _updatedMember = typeof(TEntity).GetMember(nameof(IEntity.UpdatedDate))[0];

    public static Func<TCommand, TEntity> GetCreateOperation()
    {
        return static (command) =>
        {
            var propertyCollection = UpdatePropertyCollection(command.RequestPropertyValues);

            var exprs = new List<MemberAssignment>();
            foreach (var propertyKeyPair in propertyCollection)
            {
                if (command.RequestForeignEntities.TryGetValue(propertyKeyPair.Key, out var foreignEntity))
                {
                    CreateCollection(exprs, propertyKeyPair, foreignEntity);
                    continue;
                }

                exprs.Add(
                    Expression.Bind(typeof(TEntity).GetMember(propertyKeyPair.Key)[0],
                    Expression.Constant(propertyKeyPair.Value)));
            }

            var now = Expression.Constant(DateTime.UtcNow);
            exprs.AddRange(new[]
            {
                    Expression.Bind(_idMember, Expression.Constant(Guid.NewGuid())),
                    Expression.Bind(_createdMember, now),
                    Expression.Bind(_updatedMember, now)
            });

            var create = Expression.Lambda<Func<TCommand, TEntity>>(
                Expression.MemberInit(Expression.New(typeof(TEntity)), exprs),
                Expression.Parameter(typeof(TCommand))).Compile();

            return create(command);
        };
    }

    public static Action<TEntity, TCommand> GetUpdateOperation()
    {
        return static (entity, command) =>
        {
            var propertyCollection = UpdatePropertyCollection(command.RequestPropertyValues);

            var parameter = Expression.Parameter(typeof(TEntity));

            var now = DateTime.UtcNow;

            var expressions = new List<Expression>();
            foreach (var propertyKeyPair in propertyCollection)
            {
                if (command.RequestForeignEntities.TryGetValue(propertyKeyPair.Key, out var foreignEntity))
                {
                    UpdateCollection(entity, parameter, expressions, propertyKeyPair, foreignEntity);
                    continue;
                }

                expressions.Add(CreateAssignExpression(parameter, propertyKeyPair.Key, propertyKeyPair.Value));
            }

            expressions.Add(CreateAssignExpression(parameter, _updatedMember.Name, now));

            Expression.Lambda<Action<TEntity>>(Expression.Block(expressions), parameter).Compile()(entity);
        };
    }

    private static Expression CreateAssignExpression(ParameterExpression parameter, string propertyName, object propertyValue)
    {
        return Expression.Assign(Expression.Property(parameter, propertyName), Expression.Constant(propertyValue));
    }


    private static IDictionary<string, object> UpdatePropertyCollection(IDictionary<string, object> propertyCollection)
    {
        return propertyCollection;
    }

    private static void CreateCollection(List<MemberAssignment> exprs, KeyValuePair<string, object> propertyKeyPair, Type foreignEntity)
    {
        var foreignCollectionType = typeof(List<>).MakeGenericType(foreignEntity);
        var addMethod = foreignCollectionType.GetMethod(Method.Add);
        var foreignEntities = propertyKeyPair.Value as IEnumerable<object>;

        var list = Expression.ListInit(
            Expression.New(foreignCollectionType),
            foreignEntities.Select(entity => Expression.ElementInit(
                addMethod, Expression.Constant(entity))));

        exprs.Add(Expression.Bind(typeof(TEntity).GetMember(propertyKeyPair.Key)[0], list));
    }

    private static void UpdateCollection(
        TEntity entity,
        ParameterExpression parameter,
        List<Expression> exprs,
        KeyValuePair<string, object> propertyKeyPair,
        Type foreignEntity)
    {
        var foreignType = typeof(ICollection<>).MakeGenericType(foreignEntity);
        var addMethod = foreignType.GetMethod(Method.Add);
        var foreignEntities = propertyKeyPair.Value as IEnumerable<object>;

        var member = typeof(TEntity).GetProperty(propertyKeyPair.Key).GetValue(entity) as IEnumerable<object>;
        var property = Expression.Property(parameter, propertyKeyPair.Key);

        exprs.AddRange(foreignEntities
             .Where(foreign => !member.Contains(foreign)) // better way to check?
             .Select(uforeign => Expression.Call(
                 property, addMethod, Expression.Constant(uforeign))));
    }


}

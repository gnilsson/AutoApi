using System.Linq.Expressions;

namespace AutoApi.Utility;

public static class ExpressionUtility
{
    public delegate object EmptyConstructorDelegate();

    public static EmptyConstructorDelegate CreateEmptyConstructor(Type type)
    {
        return Expression.Lambda<EmptyConstructorDelegate>(
            Expression.New(
                type.GetConstructor(Type.EmptyTypes)!)).Compile();
    }

    public delegate object ConstructorDelegate(params object[] args);

    public static ConstructorDelegate CreateConstructor(Type type, params Type[] parameters)
    {
        var constructorInfo = type.GetConstructor(parameters)!;
        var paramExpr = Expression.Parameter(typeof(object[]));

        var constructorParameters = parameters.Select((paramType, index) =>
            Expression.Convert(
                Expression.ArrayAccess(
                    paramExpr,
                    Expression.Constant(index)),
                paramType)).ToArray();

        var body = Expression.New(constructorInfo, constructorParameters);
        var constructor = Expression.Lambda<ConstructorDelegate>(body, paramExpr);
        return constructor.Compile();
    }
}

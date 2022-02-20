using System.Reflection;
using AutoApi.Defined.Descriptive;
using AutoApi.QueryRequestDefinition.Expressions;
using AutoApi.Utility;

namespace AutoApi.Defined;

public static partial class MethodFactory
{
    public static Dictionary<string, MethodInfo> QueryMethodContainer { get; }
    public static MethodInfo Contains { get; }
    public static MethodInfo CompareTo { get; }

    static MethodFactory()
    {
        var flags = BindingFlags.Public | BindingFlags.Static;

        QueryMethodContainer = new()
        {
            [QueryMethod.CallStringContains] =
            typeof(ExpressionUtility).GetMethod(nameof(QueryExpressionUtility.CallStringContains), flags)!,

            [QueryMethod.CallDateTimeCompare] =
            typeof(ExpressionUtility).GetMethod(nameof(QueryExpressionUtility.CallDateTimeCompare), flags)!
        };

        Contains = typeof(string).GetMethod(nameof(Method.Contains), new[] { typeof(string) })!;

        CompareTo = typeof(DateTime).GetMethod(Method.CompareTo, new[] { typeof(DateTime) })!;
    }
}

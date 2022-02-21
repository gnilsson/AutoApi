using System.Reflection;
using AutoApi.EntityFramework.Repository;
using GN.Toolkit;

namespace AutoApi.Rest.Defined;

public static class MethodFactory
{
    public static Func<Type, MethodInfo> GetManyAsync { get; }

    static MethodFactory()
    {
        GetManyAsync = (type) => typeof(IGeneralRepository).GetMethod(
            nameof(IGeneralRepository.GetManyAsync),
            1,
            new[] { typeof(IEnumerable<string>), typeof(CancellationToken) })!.MakeGenericMethod(type);
    }
}

using System.Reflection;
using AutoApi.EntityFramework.Repository;

namespace AutoApi.EntityFramework.Defined;

public static class MethodFactory
{
    public static Func<Type, MethodInfo> GetManyAsync { get; }

    static MethodFactory()
    {
        GetManyAsync = static (type) => typeof(IGeneralRepository).GetMethod(
            nameof(IGeneralRepository.GetManyAsync),
            1,
            new[] { typeof(IEnumerable<Guid>), typeof(CancellationToken) })!.MakeGenericMethod(type);
    }
}

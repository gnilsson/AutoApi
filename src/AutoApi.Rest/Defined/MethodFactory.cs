using System.Reflection;
using AutoApi.EntityFramework.Repository;

namespace AutoApi.Rest.Defined;

public static partial class MethodFactory
{
    public static Func<Type, MethodInfo> GetManyAsync { get; }

    static MethodFactory()
    {
        GetManyAsync = (type) => typeof(IGeneralRepository).GetMethod(
            nameof(IGeneralRepository.GetManyAsync),
            1,
            new[] { typeof(IEnumerable<Guid>), typeof(CancellationToken) })!.MakeGenericMethod(type);
    }
}

using AutoApi.Rest.Shared.Requests;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AutoApi.QueryRequestDefinition;

public static class QueryUtility
{
    public static IQueryable<TResponse> ProjectTo<TResponse, TEntity>(this IQueryable<TEntity> query, IConfigurationProvider config, params string[] expandMembers)
    {
        return expandMembers is null or { Length: 0 }
            ? query.ProjectTo<TResponse>(config)
            : query.ProjectTo<TResponse>(config, null, expandMembers);

    }

    public static IQueryable<TEntity> ApplyPaging<TEntity>(this IQueryable<TEntity> efQuery, IPaginateable pagination)
    {
        return efQuery
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize);
    }

    public static IQueryable<TEntity> IncludeBy<TEntity>(
        this IQueryable<TEntity> source,
        ICollection<string[]> details)
        where TEntity : class
    {
        foreach (var detail in details)
        {
            source = source.Include(string.Join(".", detail));
        }

        return source;
    }
}

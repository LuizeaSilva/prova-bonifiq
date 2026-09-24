using ProvaPub.Models;

namespace ProvaPub.Extensions;

public static class QueryableExtensions
{
    public const int DefaultPageSize = 10;

    public static PagedList<T> ToPagedList<T>(this IOrderedQueryable<T> query, int page, int pageSize = DefaultPageSize)
    {
        if (page < 1)
            page = 1;

        var totalCount = query.Count();

        var data = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedList<T>
        {
            HasNext = page * pageSize < totalCount,
            TotalCount = totalCount,
            Data = data
        };
    }
}

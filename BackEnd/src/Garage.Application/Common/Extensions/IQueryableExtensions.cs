using Garage.Contracts.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Garage.Application.Common.Extensions;

public static class IQueryableExtensions
{
    public static async Task<QueryResult<T>> ToQueryResult<T>(
        this IQueryable<T> dbQuery,
        int pageNumber = 1,
        int pageSize = 10,
        bool isCountOnly = false,
        string? sort = null,
        bool descending = true,
        CancellationToken ct = default
    ) where T : class
    {
        if (dbQuery == null) throw new ArgumentNullException(nameof(dbQuery));

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var count = await dbQuery.CountAsync(ct);

        // لو المطلوب count فقط
        if (isCountOnly)
        {
            return new QueryResult<T>(items: null, totalCount: count, pageNumber: pageNumber, pageSize: pageSize);
        }

        // sort + paging
        if (!string.IsNullOrWhiteSpace(sort))
            dbQuery = dbQuery.SortBy(sort!, descending);

        dbQuery = dbQuery.Page(pageNumber, pageSize);

        var data = await dbQuery.ToListAsync(ct);

        return new QueryResult<T>(items: data, totalCount: count, pageNumber: pageNumber, pageSize: pageSize);
    }

    public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize <= 0) return source;
        return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    public static IQueryable<T> SortBy<T>(this IQueryable<T> source, string propertyName, bool descending)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) return source;

        var type = typeof(T);

        // IgnoreCase + Public Instance
        var prop = type.GetProperty(
            propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
        );

        if (prop == null) return source; // أو throw لو تحب

        var parameter = Expression.Parameter(type, "x");
        var propertyAccess = Expression.MakeMemberAccess(parameter, prop);
        var keySelector = Expression.Lambda(propertyAccess, parameter);

        var methodName = descending ? "OrderByDescending" : "OrderBy";

        var resultExp = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { type, prop.PropertyType },
            source.Expression,
            Expression.Quote(keySelector)
        );

        return source.Provider.CreateQuery<T>(resultExp);
    }

    public static IQueryable<TSource> WhereIf<TSource>(
        this IQueryable<TSource> source,
        bool condition,
        Expression<Func<TSource, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }
}

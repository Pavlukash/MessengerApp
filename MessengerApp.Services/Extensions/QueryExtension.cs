using System.Linq.Expressions;
using MessengerApp.Domain.Entities.Base;
using MessengerApp.Domain.Exceptions;
using MessengerApp.Services.Contracts;
using MessengerApp.Services.Contracts.Filters;
using Microsoft.EntityFrameworkCore;

namespace MessengerApp.Services.Extensions;

public static class QueryExtension
{
    public static async Task<T> FirstOrNotFoundAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken)
    {
        var result = await query.FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            throw new MessengerAppNotFoundException();
        }

        return result;
    }
    
    public static async Task<T> FirstOrNotFoundAsync<T>(this IEnumerable<T> query, CancellationToken cancellationToken)
    {
        var result = await Task.Run(query.FirstOrDefault, cancellationToken);

        if (result is null)
        {
            throw new MessengerAppNotFoundException();
        }

        return result;
    }
    
    public static IOrderedQueryable<T> FilterOrder<T>(this IQueryable<T> query, OrderFilter orderFilter)
    {
        if (orderFilter.PropertyName == null)
        {
            return (IOrderedQueryable<T>)query;
        }

        var type = typeof(T);
        var propInfo = type.GetProperty(orderFilter.PropertyName);

        if (propInfo == null)
        {
            throw new MessengerAppNotFoundException();
        }

        var typeParams = new []
        {
            Expression.Parameter(type, string.Empty)
        };

        var method = orderFilter.IsDescending == true
            ? "OrderByDescending" 
            : "OrderBy";

        var result = (IOrderedQueryable<T>)query.Provider.CreateQuery(
            Expression.Call(
                typeof(Queryable),
                method,
                new[] { type, propInfo.PropertyType },
                query.Expression,
                Expression.Lambda(Expression.Property(typeParams[0], propInfo), typeParams))
        );

        return result;
    }
    
    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, PageFilter filter, CancellationToken cancellationToken)
    {
        var paged = new PagedList<T>
        {
            PageNum = filter.PageNum,
            PageSize = filter.PageSize,
            TotalCount = source.Count()
        };
        paged.TotalPages = (int) Math.Ceiling(paged.TotalCount / (double) paged.PageSize);
        
        paged.Items = await source
            .Skip(paged.PageNum > 1 
                ? paged.PageNum * paged.PageSize 
                : 0)
            .Take(paged.PageSize)
            .ToListAsync(cancellationToken);

        return paged;
    }
    
    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> source, string? searchValue) where T : ISearchable
    {
        return string.IsNullOrWhiteSpace(searchValue) 
            ? source 
            : source.Where(x => x.SearchValue.Contains(searchValue));
    }
}
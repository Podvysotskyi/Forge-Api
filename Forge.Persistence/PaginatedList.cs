using Forge.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Forge.Persistence;

public class PaginatedList<T> : IPaginatedList<T> where T : class
{
    public required IEnumerable<T> Items { get; init; }

    public required int Page { get; init; }

    public required int PageSize { get; init; }

    public required int Total { get; init; }

    public static PaginatedList<T> Create(IEnumerable<T> items, int page, int pageSize, int total)
    {
        return new PaginatedList<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    public static async Task<PaginatedList<T>> Create(IQueryable<T> query, int page, int itemsPerPage)
    {
        var total = await query.CountAsync();

        var items = await query.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();

        return Create(items, page, itemsPerPage, total);
    }
}

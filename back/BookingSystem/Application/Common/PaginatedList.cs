using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Application.Common;

public class PaginatedList<T>
{
    private readonly int _pageSize;

    public IReadOnlyCollection<T> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public PaginatedList(
        IReadOnlyCollection<T> items,
        int count,
        int pageNumber,
        int pageSize)
    {
        PageNumber = pageNumber;
        _pageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    // Same page, with its items converted
    public PaginatedList<TResult> Map<TResult>(Func<T, TResult> selector) =>
        new(Items.Select(selector).ToList(), TotalCount, PageNumber, _pageSize);

    // Paginates a list that has already been loaded
    public static PaginatedList<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var all = source.ToList();
        var items = all
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedList<T>(items, all.Count, pageNumber, pageSize);
    }

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);

        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(
            items,
            count,
            pageNumber,
            pageSize);
    }
}

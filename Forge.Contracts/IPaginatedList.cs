namespace Forge.Contracts;

public interface IPaginatedList<T> where T : class {
    public IEnumerable<T> Items { get; init; }

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int Total { get; init; }
}
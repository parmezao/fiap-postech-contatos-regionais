namespace ContatosRegionais.Application.ViewModels;

public static class QueryableExtensions
{
    public static PagedResult<T> ToPagedResult<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        var totalItems = query.Count();
        var data = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResult<T>(data, totalItems, pageNumber, pageSize);
    }
}
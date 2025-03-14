namespace ContatosRegionais.Application.ViewModels;

public class PagedResult<T>(IEnumerable<T> data, int totalItems, int pageNumber, int pageSize)
{
    public IEnumerable<T> Data { get; set; } = data;
    public int TotalItems { get; set; } = totalItems;
    public int TotalPages { get; set; } = (int)Math.Ceiling(totalItems / (double)pageSize);
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
}
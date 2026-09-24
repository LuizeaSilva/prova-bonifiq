namespace ProvaPub.Models;

public class PagedList<T>
{
    public List<T>? Data { get; set; }
    public int TotalCount { get; set; }
    public bool HasNext { get; set; }
}
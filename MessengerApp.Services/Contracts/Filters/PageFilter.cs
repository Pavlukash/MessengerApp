namespace MessengerApp.Services.Contracts.Filters;

public abstract class PageFilter
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
}
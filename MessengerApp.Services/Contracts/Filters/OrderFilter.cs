namespace MessengerApp.Services.Contracts.Filters;

public sealed class OrderFilter
{
    public string? PropertyName { get; set; } 
    public bool? IsDescending { get; set; }
}
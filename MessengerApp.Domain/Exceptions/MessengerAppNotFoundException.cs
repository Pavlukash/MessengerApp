using Microsoft.AspNetCore.Http;

namespace MessengerApp.Domain.Exceptions;

public class MessengerAppNotFoundException : MessengerAppException
{
    public MessengerAppNotFoundException(string? message = null) : base(message) { }
    
    public override int GetStatusCode()
    {
        return StatusCodes.Status404NotFound;
    }
}
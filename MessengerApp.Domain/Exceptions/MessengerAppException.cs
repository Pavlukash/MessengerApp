using Microsoft.AspNetCore.Http;

namespace MessengerApp.Domain.Exceptions;

public class MessengerAppException : Exception
{
    public MessengerAppException(string? message = null) : base(message) { }
    public virtual int GetStatusCode()
    {
        return StatusCodes.Status500InternalServerError;
    }
}
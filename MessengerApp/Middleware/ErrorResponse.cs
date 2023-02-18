using Newtonsoft.Json;

namespace SignalRTraining.Middleware;

public sealed class ErrorResponse
{
    public int StatusCode { get; init; }
    public string? ErrorMessage { get; init; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
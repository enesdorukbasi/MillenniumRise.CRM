using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Presentation.Server.Common.ApiResult;

public class ApiResult
{
    public required int StatusCode { get; init; }
    public string? Code { get; init; }
    public ProblemDetails? Fail { get; init; }

    [JsonIgnore]
    public bool IsSuccess => Fail is null;
}

public sealed class ApiResult<T> : ApiResult
{
    public T? Data { get; init; }
}

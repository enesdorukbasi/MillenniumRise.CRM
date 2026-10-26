using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Presentation.Server.Common.ApiResult;

public static class ApiResultFactory
{
    public static ApiResult Success(HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ApiResult
        {
            StatusCode = statusCode,
        };
    }

    public static ApiResult Success<T>(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ApiResult<T>
        {
            StatusCode = statusCode,
            Data = data,
        };
    }

    public static ApiResult Created<T>(T data)
    {
        return Success(data, HttpStatusCode.Created);
    }

    public static ApiResult Fail(string title, string? detail, HttpStatusCode statusCode, PathString instance, string? code = null)
    {
        var problemDetails = new ProblemDetails
        {
            Type = statusCode switch
            {
                HttpStatusCode.BadRequest => "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.1",
                HttpStatusCode.Unauthorized => "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.2",
                HttpStatusCode.Forbidden => "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.4",
                HttpStatusCode.NotFound => "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.5",
                HttpStatusCode.Conflict => "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.10",
                HttpStatusCode.UnprocessableEntity => "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.21",
                HttpStatusCode.TooManyRequests => "https://www.rfc-editor.org/rfc/rfc6585#section-4",
                _ => "https://www.rfc-editor.org/rfc/rfc9110#section-15.6.1"
            },
            Status = statusCode.GetHashCode(),
            Title = title,
            Detail = detail,
            Instance = instance
        };
        return new ApiResult
        {
            StatusCode = statusCode,
            Fail = problemDetails
        };
    }

    public static ApiResult ValidationFail(IEnumerable<string> errors, PathString instance, string? detail = null)
    {
        var problemDetails = new ProblemDetails
        {
            Type = "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.1",
            Status = HttpStatusCode.BadRequest.GetHashCode(),
            Title = "Doğrulama hatası",
            Detail = detail ?? "Lütfen hatalar alanındaki bilgileri kontrol edin.",
            Instance = instance
        };

        problemDetails.Extensions["errors"] = errors
            .Where(error => !string.IsNullOrWhiteSpace(error))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return new ApiResult
        {
            StatusCode = HttpStatusCode.BadRequest,
            Fail = problemDetails
        };
    }

    public static IActionResult ToActionResult(this ApiResult apiResult)
    {
        return apiResult.StatusCode switch
        {
            HttpStatusCode.Created => new CreatedResult(string.Empty, apiResult),
            HttpStatusCode.NoContent => new NoContentResult(),
            _ => new ObjectResult(apiResult)
            {
                StatusCode = apiResult.StatusCode.GetHashCode()
            }
        };
    }
}

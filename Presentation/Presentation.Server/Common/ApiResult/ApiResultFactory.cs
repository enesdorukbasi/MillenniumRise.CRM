using Microsoft.AspNetCore.Mvc;

namespace Presentation.Server.Common.ApiResult;

public static class ApiResultFactory
{
    public static IActionResult Success(int statusCode = StatusCodes.Status200OK, string? code = null)
    {
        return new ApiResult
        {
            StatusCode = statusCode,
            Code = code,
        }.ToActionResult();
    }

    public static IActionResult Success<T>(T data, int statusCode = StatusCodes.Status200OK, string? code = null)
    {
        return new ApiResult<T>
        {
            StatusCode = statusCode,
            Code = code,
            Data = data,
        }.ToActionResult();
    }

    public static IActionResult Created<T>(T data, string? code = null)
    {
        return Success(data, StatusCodes.Status201Created, code);
    }

    public static IActionResult Fail(string title, string? detail, int statusCode, PathString instance, string? code = null)
    {
        var problemDetails = new ProblemDetails
        {
            Type = statusCode switch
            {
                StatusCodes.Status400BadRequest => "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.1",
                StatusCodes.Status401Unauthorized => "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.2",
                StatusCodes.Status403Forbidden => "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.4",
                StatusCodes.Status404NotFound => "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.5",
                StatusCodes.Status409Conflict => "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.10",
                StatusCodes.Status422UnprocessableEntity => "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.21",
                StatusCodes.Status429TooManyRequests => "https://www.rfc-editor.org/rfc/rfc6585#section-4",
                _ => "https://www.rfc-editor.org/rfc/rfc9110#section-15.6.1"
            },
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = instance
        };

        return new ApiResult
        {
            StatusCode = statusCode,
            Code = code,
            Fail = problemDetails
        }.ToActionResult();
    }

    public static IActionResult ValidationFail(IEnumerable<string> errors, PathString instance, string? detail = null)
    {
        var problemDetails = new ProblemDetails
        {
            Type = "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.1",
            Status = StatusCodes.Status400BadRequest,
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
            StatusCode = StatusCodes.Status400BadRequest,
            Code = "validation_error",
            Fail = problemDetails
        }.ToActionResult();
    }

    private static IActionResult ToActionResult(this ApiResult apiResult)
    {
        return apiResult.StatusCode switch
        {
            201 => new CreatedResult(string.Empty, apiResult),
            204 => new NoContentResult(),
            _ => new ObjectResult(apiResult)
            {
                StatusCode = apiResult.StatusCode
            }
        };
    }
}

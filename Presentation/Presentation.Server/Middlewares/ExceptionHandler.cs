using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Presentation.Server.Common.ApiResult;
using System.Diagnostics;
using System.Net;

namespace Presentation.Server.Middlewares;

public sealed class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ApiResult apiResult;

        if (exception is AppValidationException validationException)
        {
            apiResult = ApiResultFactory.ValidationFail(validationException.Errors, httpContext.Request.Path, validationException.Message);
        }
        else
        {
            var (statusCode, title) = exception switch
            {
                UnauthorizedException => (HttpStatusCode.Unauthorized, "Kimlik doğrulama gerekli"),
                ForbiddenException => (HttpStatusCode.Forbidden, "Erişim reddedildi"),
                NotFoundException or KeyNotFoundException => (HttpStatusCode.NotFound, "Kayıt bulunamadı"),
                ConflictException => (HttpStatusCode.Conflict, "Çakışma"),
                BusinessRuleException => (HttpStatusCode.UnprocessableEntity, "İş kuralı ihlali"),
                _ => (HttpStatusCode.InternalServerError, "Beklenmeyen bir hata oluştu")
            };

            var detail = statusCode >= HttpStatusCode.InternalServerError
                ? "İşlem sırasında beklenmeyen bir hata oluştu."
                : exception.Message;

            apiResult = ApiResultFactory.Fail(title, detail, statusCode, httpContext.Request.Path);
        }

        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        apiResult.Fail?.Extensions.TryAdd("traceId", traceId);

        if (apiResult.StatusCode >= HttpStatusCode.InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception for {Method} {Path}. TraceId: {TraceId}", httpContext.Request.Method, httpContext.Request.Path, traceId);
        }
        else
        {
            logger.LogWarning("Request {Method} {Path} failed with {StatusCode} ({ExceptionType}). TraceId: {TraceId}", httpContext.Request.Method, httpContext.Request.Path, apiResult.StatusCode, exception.GetType().Name, traceId);
        }

        httpContext.Response.StatusCode = apiResult.StatusCode.GetHashCode();
        await httpContext.Response.WriteAsJsonAsync(apiResult, cancellationToken);
        return true;
    }
}

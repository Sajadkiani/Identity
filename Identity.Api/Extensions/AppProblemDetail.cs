using System;
using System.Text.Json;
using System.Threading.Tasks;
using Identity.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ApplicationException = Identity.Infrastructure.Exceptions.ApplicationException;

namespace Identity.Api.Extensions;

public static class AppProblemDetail
{
    public static void UseAppProblemDetail(this IApplicationBuilder app)
    {
        app.UseMiddleware<ProblemHandlingMiddleware>();
    }
}

public class ProblemHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ProblemHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var logger = context.RequestServices.GetService<ILogger<ProblemHandlingMiddleware>>();
            logger?.LogError(ex, "ProblemHandlingMiddleware Exception");

            var problem = MapExceptionToProblemDetails(context, ex);

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problem.Status ?? 500;

            var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }

    private ProblemDetails MapExceptionToProblemDetails(HttpContext context, Exception ex)
    {
        ProblemDetails problem;

        switch (ex)
        {
            case ApplicationException.NotFound badReq:
                problem = new ProblemDetails
                {
                    Title = nameof(StatusCodes.Status400BadRequest),
                    Detail = badReq.Message,
                    Status = StatusCodes.Status404NotFound,
                    Instance = context.Request.Path,
                };
                break;

            case ApplicationException.Internal appEx:
                problem = new ProblemDetails
                {
                    Title = nameof(StatusCodes.Status500InternalServerError),
                    Detail = appEx.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = context.Request.Path
                };
                break;
            
            case ApplicationException.BadRequest appEx:
                problem = new ProblemDetails
                {
                    Title = nameof(StatusCodes.Status500InternalServerError),
                    Detail = appEx.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = context.Request.Path
                };
                break;

            default:
                problem = new ProblemDetails
                {
                    Title = nameof(StatusCodes.Status500InternalServerError),
                    Detail = AppMessages.InternalError.message,
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = context.Request.Path
                };
                break;
        }

        problem.Extensions["traceId"] = context.TraceIdentifier;
        return problem;
    }
}
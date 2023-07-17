    using System;
using Hellang.Middleware.ProblemDetails;
using Identity.Api.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ApplicationException = Identity.Api.Infrastructure.Exceptions.ApplicationException;

namespace Identity.Api.Infrastructure.Extensions;

public static class AppProblemDetail
{
    public static void UseAppProblemDetail(this IApplicationBuilder app)
    {
        app.UseProblemDetails();
    }
    
    public static void AddAppProblemDetail(this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        services.AddProblemDetails(options =>
        {
            // Only include exception details in a development environment. There's really no need
            // to set this as it's the default behavior. It's just included here for completeness :)
            options.IncludeExceptionDetails = (ctx, ex) => environment.IsDevelopment();

            // This will map UserNotFoundException to the 404 Not Found status code and return custom problem details.
            options.Map<ApplicationException.Internal>(ex => new ProblemDetails
            {
                Title = AppMessages.InternalError.message,
                Status = StatusCodes.Status500InternalServerError,
                Detail = ex.Message,
            });
            
            options.Map<ApplicationException.Unauthorized>(ex => new ProblemDetails
            {
                Title = AppMessages.Unauthenticated.message,
                Status = StatusCodes.Status403Forbidden,
                Detail = ex.Message,
            });
            
            
            options.Map<ApplicationException.NotFound>(ex => new ProblemDetails
            {
                Title = AppMessages.NotFound.message,
                Status = StatusCodes.Status404NotFound,
                Detail = ex.Message,
            });

            // This will map NotImplementedException to the 501 Not Implemented status code.
            options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);

            // You can configure the middleware to re-throw certain types of exceptions, all exceptions or based on a predicate.
            // This is useful if you have upstream middleware that  needs to do additional handling of exceptions.
            // options.Rethrow<NotSupportedException>();

            // You can configure the middleware to ingore any exceptions of the specified type.
            // This is useful if you have upstream middleware that  needs to do additional handling of exceptions.
            // Note that unlike Rethrow, additional information will not be added to the exception.
            options.Ignore<DivideByZeroException>();

            // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
            // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        });
    }
}
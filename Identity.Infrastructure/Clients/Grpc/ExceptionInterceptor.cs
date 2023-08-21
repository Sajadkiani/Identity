using Grpc.Core;
using Grpc.Core.Interceptors;
using Identity.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using ApplicationException = Identity.Infrastructure.Exceptions.ApplicationException;

namespace Identity.Infrastructure.Clients.Grpc;

public class ExceptionInterceptor: Interceptor
{
    private readonly ILogger<ExceptionInterceptor> logger;
 
    public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
    {
        this.logger = logger;
    }
     
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (ApplicationException.Unauthorized exception)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, AppMessages.Unauthenticated.message));
        }
        catch (Exception exception)
        {
            throw new RpcException(new Status(StatusCode.Internal, AppMessages.InternalError.message));
        }
    }
}
using System;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Identity.Api.Application.Queries.Users;
using Identity.Infrastructure.MtuBus;
using IdentityGrpcServer;

namespace Identity.Api.Grpc;

public class AuthGrpcService : IdentityGrpc.IdentityGrpcBase
{
    private readonly IMapper mapper;
    private readonly IDomainEventDispatcher eventBus;

    public AuthGrpcService(
        IMapper mapper,
        IDomainEventDispatcher eventBus
    )
    {
        this.mapper = mapper;
        this.eventBus = eventBus;
    }

    public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
            var token = await eventBus.SendAsync(new RefreshTokenQuery { RefreshToken = request.RefreshToken});
            return mapper.Map<RefreshTokenResponse>(token);
            // var token = await eventBus.SendMediator(new RefreshTokenQuery { RefreshToken = request.RefreshToken});
            // return mapper.Map<RefreshTokenResponse>(token);

            return null;
    }
}
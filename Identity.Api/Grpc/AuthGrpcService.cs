using System;
using System.Threading.Tasks;
using AutoMapper;
using EventBus.Abstractions;
using Grpc.Core;
using Identity.Api.Application.Queries.Users;
using IdentityGrpcServer;

namespace Identity.Api.Grpc;

public class AuthGrpcService : IdentityGrpc.IdentityGrpcBase
{
    private readonly IMapper mapper;
    private readonly IEventBus eventBus;

    public AuthGrpcService(
        IMapper mapper,
        IEventBus eventBus
    )
    {
        this.mapper = mapper;
        this.eventBus = eventBus;
    }

    public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
            var token = await eventBus.SendMediator(new RefreshTokenQuery { RefreshToken = request.RefreshToken});
            return mapper.Map<RefreshTokenResponse>(token);
    }
}
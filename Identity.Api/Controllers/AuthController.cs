using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using EventBus.Abstractions;
using Identity.Api.Application.Commands.Users;
using Identity.Api.Application.Queries.Users;
using Identity.Api.ViewModels;
using Identity.Infrastructure.Extensions.Options;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Identity.Api.Controllers;

[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly IMemoryCache cache;
    private readonly AppOptions.Jwt jwt;
    private readonly IDomainEventDispatcher eventHandler;
    private readonly ILogger<AuthController> logger;

    public AuthController(
        IMapper mapper,
        IMemoryCache cache,
        AppOptions.Jwt jwt,
        IDomainEventDispatcher eventHandler,
        ILogger<AuthController> logger
    )
    {
        this.mapper = mapper;
        this.cache = cache;
        this.jwt = jwt;
        this.eventHandler = eventHandler;
        this.logger = logger;
    }
    
    [HttpPost("~/connect/token"), IgnoreAntiforgeryToken, Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request.IsPasswordGrantType())
        {
            return await SignInPasswordGrantAsync(request);
        }
        else if (request.IsRefreshTokenGrantType())
        {
            return await SignInRefreshTokenGrantAsync(request);
        }

        throw new NotImplementedException("The specified grant type is not implemented.");
    }

    private async Task<IActionResult> SignInPasswordGrantAsync(OpenIddictRequest request)
    {
        var user = await eventHandler.SendMediator(new GetUserByUserNameQuery(request.Username));
        if (user == null)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string>
            {
                //TODO: proper error message
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                    "The username/password couple is invalid."
            });

            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var succeeded =
            await eventHandler.SendMediator(
                new CheckPasswordSignInCommand(request.Username, request.Password, true));
        if (!succeeded)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                    "The username/password couple is invalid."
            });

            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // Create the claims-based identity that will be used by OpenIddict to generate tokens.
        return AddClaims(user, request);
    }

    private async Task<IActionResult> SignInRefreshTokenGrantAsync(OpenIddictRequest request)
    {
        // Retrieve the claims principal stored in the refresh token.
        var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        // Retrieve the user profile corresponding to the refresh token.
        var user = await eventHandler.SendMediator(new GetUserByUserNameQuery(request.Username));
        if (user == null)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string>
            {
                //TODO: proper error message
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                    "The username/password couple is invalid."
            });

            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var succeeded =
            await eventHandler.SendMediator(
                new CheckPasswordSignInCommand(request.Username, request.Password, true));
        if (!succeeded)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                    "The username/password couple is invalid."
            });

            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var identity = new ClaimsIdentity(result.Principal.Claims,
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        // Override the user claims present in the principal in case they changed since the refresh token was issued.
        identity.SetClaim(OpenIddictConstants.Claims.Subject, user.Id)
            .SetClaim(OpenIddictConstants.Claims.Email, user.Email)
            .SetClaim(OpenIddictConstants.Claims.Name, user.UserName);

        identity.SetDestinations(GetDestinations);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private IActionResult AddClaims(AuthViewModel.GetUserByUserNameOutput user, OpenIddictRequest request)
    {
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        // Add the claims that will be persisted in the tokens.
        identity.SetClaim(OpenIddictConstants.Claims.Subject, user.UserName)
            .SetClaim(OpenIddictConstants.Claims.Name, user.Name)
            .SetClaim(OpenIddictConstants.Claims.FamilyName, user.Family)
            .SetClaim(OpenIddictConstants.Claims.Gender, user.Gender.ToString())
            .SetClaim(OpenIddictConstants.Claims.Email, user.Email);

        // Note: in this sample, the granted scopes match the requested scope
        // but you may want to allow the user to uncheck specific scopes.
        // For that, simply restrict the list of scopes before calling SetScopes.
        identity.SetScopes(request.GetScopes());
        identity.SetDestinations(GetDestinations);

        // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
        var signIn = SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        return signIn;
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        switch (claim.Type)
        {
            case OpenIddictConstants.Claims.Name:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Profile))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            case OpenIddictConstants.Claims.Email:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Email))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            case OpenIddictConstants.Claims.Role:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Roles))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return OpenIddictConstants.Destinations.AccessToken;
                yield break;
        }
    }
}
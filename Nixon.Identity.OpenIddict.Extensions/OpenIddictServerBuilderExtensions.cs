using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Server;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace Nixon.Identity.OpenIddict.Extensions;

[SuppressMessage("ReSharper", "ConvertToExtensionBlock")]
public static class OpenIddictServerBuilderExtensions
{
    public static OpenIddictServerBuilder AllowRefreshTokenFlow(this OpenIddictServerBuilder builder, TimeSpan refreshTokenLifetime)
    {
        return builder
            .AllowRefreshTokenFlow()
            .SetRefreshTokenLifetime(refreshTokenLifetime);
    }
    
    public static OpenIddictServerBuilder AddScopedTokenRequestHandler<THandler>(this OpenIddictServerBuilder builder)
        where THandler : class, IOpenIddictServerHandler<HandleTokenRequestContext>
    {
        return builder
            .AddEventHandler<HandleTokenRequestContext>(x => x .UseScopedHandler<THandler>());
    }

    public static OpenIddictServerBuilder AllowCustomFlows(this OpenIddictServerBuilder builder, IEnumerable<string> customFlows)
    {
        foreach (var customFlow in customFlows)
        {
            builder.AllowCustomFlow(customFlow);
        }
        
        return builder;
    }

    public static OpenIddictServerBuilder AddSigningDevelopmentCertificateOrSigningKey(
        this OpenIddictServerBuilder builder, 
        IHostEnvironment environment,
        Func<SecurityKey> keyFactory)
    {
        if (environment.IsDevelopment())
        {
            builder.AddDevelopmentSigningCertificate();
        }
        else
        {
            builder.AddSigningKey(keyFactory());
        }
        
        return builder;
    }
}
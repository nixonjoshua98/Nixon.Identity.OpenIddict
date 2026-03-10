using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nixon.Identity.OpenIddict.Extensions;
using Nixon.Identity.OpenIddict.Template.Configuration;
using OpenIddict.Server;

namespace Nixon.Identity.OpenIddict.Template.Builders;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public sealed class OpenIddictFrameworkServerBuilder(IOpenIddictFrameworkConfiguration configuration)
{
    private Action<OpenIddictServerBuilder>? _configureAction;

    private bool _hasSigningKey;

    public void Configure(OpenIddictServerBuilder builder)
    {
        // Only add a development signing key if no signing key
        // has been specified.
        if (!_hasSigningKey)
        {
            builder.AddDevelopmentSigningCertificate();
        }

        builder.AddEncryptionKey(configuration.EncryptionSecurityKey);

        builder.UseDataProtection();

        builder.SetIssuer(configuration.Issuer);
        builder.SetAccessTokenLifetime(TimeSpan.FromDays(7));

        builder.AllowRefreshTokenFlow(TimeSpan.FromDays(30));
        builder.AllowAuthorizationCodeFlow();
        builder.AllowCustomFlows(configuration.AllAllowedGrantTypes);
                
        builder.SetTokenEndpointUris("connect/token");
        builder.SetAuthorizationEndpointUris("connect/authorize");
                
        builder.UseAspNetCore(asp => asp
            .EnableAuthorizationEndpointPassthrough()
        );
                
        _configureAction?.Invoke(builder);   
    }
    
    
    public OpenIddictFrameworkServerBuilder AddSigningKey(SecurityKey key)
    {
        _configureAction += builder => builder
            .AddSigningKey(key);

        _hasSigningKey = true;
        
        return this;
    }
    
    public OpenIddictFrameworkServerBuilder AddScopedProcessErrorHandler<THandler>() 
        where THandler : class, IOpenIddictServerHandler<OpenIddictServerEvents.ProcessErrorContext>
    {
        _configureAction += builder => builder
            .AddEventHandler<OpenIddictServerEvents.ProcessErrorContext>(x => x.UseScopedHandler<THandler>());
        
        return this;
    }
    
    public OpenIddictFrameworkServerBuilder AddScopedTokenRequestHandler<THandler>() 
        where THandler : class, IOpenIddictServerHandler<OpenIddictServerEvents.HandleTokenRequestContext>
    {
        _configureAction += builder => builder
            .AddEventHandler<OpenIddictServerEvents.HandleTokenRequestContext>(x => x.UseScopedHandler<THandler>());
        
        return this;
    }
}
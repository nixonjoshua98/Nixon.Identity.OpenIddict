using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nixon.Identity.OpenIddict.Extensions;
using Nixon.Identity.OpenIddict.Template.BackgroundService;
using Nixon.Identity.OpenIddict.Template.Builders;
using Nixon.Identity.OpenIddict.Template.Configuration;

namespace Nixon.Identity.OpenIddict.Template.Extensions;

public static class ServiceCollectionExtensions
{
    private static void AddCoreServices(IServiceCollection services, OpenIddictIdentityServerConfiguration configuration)
    {
        services.AddHostedService<ApplicationRegistrationBackgroundService>();
        
        services.TryAddSingleton(configuration);
    }
    
    public static IServiceCollection AddOpenIddictIdentityServer<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        Action<OpenIdDictIdentityServerBuilder>? configure = null)
    where TContext : DbContext
    {
        var loadedConfiguration = OpenIddictIdentityServerConfiguration.ReadFrom(configuration);
        
        var templateBuilder = new OpenIdDictIdentityServerBuilder();
        
        configure?.Invoke(templateBuilder);
        
        AddCoreServices(services, loadedConfiguration);
        
        services.AddOpenIddict()
            .AddCore(core =>
            {
                core.UseEntityFrameworkCore<TContext>();
            })
            .AddServer(server =>
            {
                server.AddDevelopmentSigningCertificate();
                server.AddEncryptionKey(loadedConfiguration.EncryptionSecurityKey);

                server.UseDataProtection();

                server.SetIssuer(loadedConfiguration.Issuer);
                server.SetAccessTokenLifetime(TimeSpan.FromDays(7));

                server.AllowRefreshTokenFlow(TimeSpan.FromDays(30));
                server.AllowAuthorizationCodeFlow();
                server.AllowCustomFlows(loadedConfiguration.AllAllowedGrantTypes);
                
                server.SetTokenEndpointUris("connect/token");
                server.SetAuthorizationEndpointUris("connect/authorize");
                
                server.UseAspNetCore(builder => builder
                    .EnableAuthorizationEndpointPassthrough()
                );
                
                templateBuilder.Server.ConfigureAction?.Invoke(server);
            })
            .AddClient(client =>
            {
                client.SetRedirectionEndpointUris("connect/redirect");

                client.AllowAuthorizationCodeFlow();

                client.UseDataProtection();
                client.UseSystemNetHttp();

                client.AddValidateIssuerGoogleWorkaround();
                
                client.AddEncryptionKey(loadedConfiguration.EncryptionSecurityKey);
                client.AddDevelopmentSigningCertificate();
                
                client.UseAspNetCore(builder => builder
                    .DisableDevelopmentTransportSecurityRequirement(environment)
                );
            })
            .AddValidation(validation =>
            {
                validation.SetIssuer(loadedConfiguration.Issuer);

                validation.UseAspNetCore();
                validation.UseLocalServer();
                validation.UseSystemNetHttp();
                validation.UseDataProtection();
            });
        
        return services;
    }
}
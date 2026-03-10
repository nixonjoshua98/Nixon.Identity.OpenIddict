using System.Diagnostics.CodeAnalysis;
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

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class ServiceCollectionExtensions
{
    private static void AddCoreServices<TConfiguration>(IServiceCollection services, TConfiguration configuration)
        where TConfiguration : class, IOpenIddictFrameworkConfiguration
    {
        services.AddHostedService<ApplicationRegistrationBackgroundService>();
        
        services.TryAddSingleton<IOpenIddictFrameworkConfiguration>(configuration);
        
        services.TryAddSingleton(configuration);
    }

    private static T LoadConfiguration<T>(IConfiguration configuration, string sectionName)
        where T : IOpenIddictFrameworkConfiguration, new()
    {
        var section = configuration.GetRequiredSection(sectionName);

        var loaded = new T();
        
        section.Bind(loaded);
        
        return loaded;
    }
    
    public static IServiceCollection AddOpenIddictIdentityServer<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        Action<OpenIddictFrameworkBuilder<OpenIddictFrameworkConfiguration>>? configure = null)
        where TContext : DbContext
    {
        return AddOpenIddictIdentityServer<TContext, OpenIddictFrameworkConfiguration>(
            services, configuration, environment, configure
        );
    }

    public static IServiceCollection AddOpenIddictIdentityServer<TContext, TConfiguration>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        Action<OpenIddictFrameworkBuilder<TConfiguration>>? configure = null)
        where TContext : DbContext
        where TConfiguration : class, IOpenIddictFrameworkConfiguration, new()
    {
        return AddOpenIddictIdentityServer<TContext, TConfiguration>(
            services, 
            configuration, 
            environment, 
            "OpenIddict", 
            configure
        );
    }

    public static IServiceCollection AddOpenIddictIdentityServer<TContext, TConfiguration>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string sectionName,
        Action<OpenIddictFrameworkBuilder<TConfiguration>>? configure = null)
        where TContext : DbContext
        where TConfiguration : class, IOpenIddictFrameworkConfiguration, new()
    {
        var loadedConfiguration = LoadConfiguration<TConfiguration>(configuration, sectionName);
        
        var identityServerBuilder = new OpenIddictFrameworkBuilder<TConfiguration>(loadedConfiguration, environment);
        
        configure?.Invoke(identityServerBuilder);
        
        AddCoreServices(services, loadedConfiguration);
        
        services.AddOpenIddict()
            .AddCore(core =>
            {
                core.UseEntityFrameworkCore<TContext>();
            })
            .AddServer(identityServerBuilder.Server.Configure)
            .AddClient(identityServerBuilder.Client.Configure)
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
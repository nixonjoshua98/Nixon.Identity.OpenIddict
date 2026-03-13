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
    extension(IServiceCollection services)
    {
        public IServiceCollection AddOpenIddictFramework<TContext>(IConfiguration configuration,
            IHostEnvironment environment,
            Action<OpenIddictFrameworkBuilder<OpenIddictFrameworkConfiguration>>? configure = null)
            where TContext : DbContext
        {
            return AddOpenIddictFramework<TContext, OpenIddictFrameworkConfiguration>(
                services, 
                configuration, 
                environment, 
                configure
            );
        }

        public IServiceCollection AddOpenIddictFramework<TContext, TConfiguration>(IConfiguration configuration,
            IHostEnvironment environment,
            Action<OpenIddictFrameworkBuilder<TConfiguration>>? configure = null)
            where TContext : DbContext
            where TConfiguration : class, IOpenIddictFrameworkConfiguration, new()
        {
            return AddOpenIddictFramework<TContext, TConfiguration>(
                services, 
                configuration, 
                environment, 
                "OpenIddict", 
                configure
            );
        }

        public IServiceCollection AddOpenIddictFramework<TContext, TConfiguration>(IConfiguration configuration,
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
}
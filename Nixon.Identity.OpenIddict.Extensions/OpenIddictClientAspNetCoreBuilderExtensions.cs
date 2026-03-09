using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nixon.Identity.OpenIddict.Extensions;

public static class OpenIddictClientAspNetCoreBuilderExtensions
{
    public static OpenIddictClientAspNetCoreBuilder DisableDevelopmentTransportSecurityRequirement(
        this OpenIddictClientAspNetCoreBuilder builder, 
        IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            builder.DisableTransportSecurityRequirement();
        }
        
        return builder;
    }
}
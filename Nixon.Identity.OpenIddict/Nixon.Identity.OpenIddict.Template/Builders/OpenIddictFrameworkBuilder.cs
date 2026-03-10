using Microsoft.Extensions.Hosting;
using Nixon.Identity.OpenIddict.Template.Configuration;

namespace Nixon.Identity.OpenIddict.Template.Builders;

public sealed class OpenIddictFrameworkBuilder<TConfiguration>(TConfiguration configuration, IHostEnvironment environment)
    where TConfiguration : IOpenIddictFrameworkConfiguration
{
    public readonly TConfiguration Configuration = configuration;
    public readonly OpenIddictFrameworkServerBuilder Server = new(configuration);
    public readonly OpenIddictFrameworkClientBuilder Client = new(configuration, environment);
}
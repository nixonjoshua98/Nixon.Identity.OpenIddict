using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Nixon.Identity.OpenIddict.Extensions;

public static class OpenIddictClientBuilderExtensions
{
    public static OpenIddictClientBuilder AddSigningDevelopmentCertificateOrSigningKey(
        this OpenIddictClientBuilder builder, 
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
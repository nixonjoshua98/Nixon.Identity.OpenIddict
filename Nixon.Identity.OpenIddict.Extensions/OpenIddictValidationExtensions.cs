using Microsoft.Extensions.DependencyInjection;

namespace Nixon.Identity.OpenIddict.Extensions;

public static class OpenIddictValidationExtensions
{
    public static OpenIddictValidationBuilder SetClient(OpenIddictValidationBuilder builder, string clientId, string clientSecret)
    {
        return builder
            .SetClientId(clientId)
            .SetClientSecret(clientSecret);
    }
}
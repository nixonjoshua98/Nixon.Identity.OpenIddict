using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using OpenIddict.Client.WebIntegration;

namespace Nixon.Identity.OpenIddict.Extensions;

public static class OpenIddictClientBuilderExtensions
{
    public static OpenIddictClientBuilder AddValidateIssuerGoogleWorkaround(this OpenIddictClientBuilder builder)
    {
        // https://github.com/openiddict/openiddict-core/issues/2428#issuecomment-4025856727
        
        builder.RemoveEventHandler(OpenIddictClientHandlers.ValidateIssuerParameter.Descriptor);

        builder.AddEventHandler<OpenIddictClientEvents.ProcessAuthenticationContext>(handler =>
        {
            handler.Import(OpenIddictClientHandlers.ValidateIssuerParameter.Descriptor);
            handler.SetType(OpenIddictClientHandlerType.Custom);

            handler.UseInlineHandler(context =>
            {
                // To help mitigate mix-up attacks, the identity of the issuer can be returned by
                // authorization servers that support it as part of the "iss" parameter, which
                // allows comparing it to the issuer in the state token. Depending on the selected
                // response_type, the same information could be retrieved from the identity token
                // that is expected to contain an "iss" claim containing the issuer identity.
                //
                // This handler eagerly validates the "iss" parameter if the authorization server
                // is known to support it (and automatically rejects the request if it doesn't).
                // Validation based on the identity token is performed later in the pipeline.
                //
                // See https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics-19#section-4.4
                // for more information.
                var issuer = (string?)context.Request[OpenIddictConstants.Parameters.Iss];

                if (context.Configuration.AuthorizationResponseIssParameterSupported is not true &&
                    (context.Registration.ProviderType is not OpenIddictClientWebIntegrationConstants.ProviderTypes
                         .Google ||
                     string.IsNullOrEmpty(issuer)))
                {
                    // Reject authorization responses containing an "iss" parameter if the configuration
                    // doesn't indicate this parameter is supported, as recommended by the specification.
                    // See https://datatracker.ietf.org/doc/html/draft-ietf-oauth-iss-auth-resp-05#section-2.4
                    // for more information.
                    if (context.Registration.ProviderType is not OpenIddictClientWebIntegrationConstants.ProviderTypes
                            .Google &&
                        !string.IsNullOrEmpty(issuer))
                    {
                        context.Reject(
                            error: OpenIddictConstants.Errors.InvalidRequest,
                            description: OpenIddictResources.FormatID2120(OpenIddictConstants.Parameters.Iss,
                                OpenIddictConstants.Metadata.AuthorizationResponseIssParameterSupported),
                            uri: OpenIddictResources.FormatID8000(OpenIddictResources.ID2120));
                    }
                }
                else
                {
                    // Reject authorization responses that don't contain the "iss" parameter
                    // if the server configuration indicates this parameter should be present.
                    if (string.IsNullOrEmpty(issuer))
                    {
                        context.Reject(
                            error: OpenIddictConstants.Errors.InvalidRequest,
                            description: OpenIddictResources.FormatID2029(OpenIddictConstants.Parameters.Iss),
                            uri: OpenIddictResources.FormatID8000(OpenIddictResources.ID2029));

                        return ValueTask.CompletedTask;
                    }

                    // If the two values don't match, this may indicate a mix-up attack attempt.
                    if (Uri.TryCreate(issuer, UriKind.Absolute, out Uri? uri) && uri == context.Registration.Issuer)
                        return ValueTask.CompletedTask;

                    context.Reject(
                        error: OpenIddictConstants.Errors.InvalidRequest,
                        description: OpenIddictResources.FormatID2119(OpenIddictConstants.Parameters.Iss),
                        uri: OpenIddictResources.FormatID8000(OpenIddictResources.ID2119));
                }

                return ValueTask.CompletedTask;
            });
        });
        
        return builder;
    }
}
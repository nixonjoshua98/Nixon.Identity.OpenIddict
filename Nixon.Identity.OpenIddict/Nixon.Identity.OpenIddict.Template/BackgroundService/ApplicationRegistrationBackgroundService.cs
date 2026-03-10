using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nixon.Identity.OpenIddict.Extensions;
using Nixon.Identity.OpenIddict.Template.Configuration;
using OpenIddict.Abstractions;

namespace Nixon.Identity.OpenIddict.Template.BackgroundService;

    internal sealed class ApplicationRegistrationBackgroundService(
        OpenIddictIdentityServerConfiguration configuration,
        IServiceProvider serviceProvider
    ) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var scope = serviceProvider.CreateAsyncScope();

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            foreach (var application in configuration.Applications)
            {
                var descriptor = CreateApplicationDescriptor(
                    application.ClientId,
                    application.AllowedGrantTypes,
                    []
                );
                
                await manager.CreateOrUpdateAsync(descriptor, cancellationToken);
            }
        }

        private static OpenIddictApplicationDescriptor CreateApplicationDescriptor(
            string clientId,
            string[] grantTypes,
            IEnumerable<string> redirectUris
        )
        {
            var application = new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                ClientType = OpenIddictConstants.ClientTypes.Public,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Authorization,

                    OpenIddictConstants.Permissions.ResponseTypes.Code,

                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken
                }
            };

            application.AddGrantTypePermissions(grantTypes);
            application.AddRedirectUris(redirectUris);

            return application;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
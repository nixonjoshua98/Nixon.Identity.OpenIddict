using OpenIddict.Abstractions;

namespace Nixon.Identity.OpenIddict.Extensions;

public static class OpenIddictApplicationManagerExtensions
{
    public static async Task<IOpenIddictApplicationManager> CreateOrUpdateAsync(
        this IOpenIddictApplicationManager manager,
        OpenIddictApplicationDescriptor descriptor,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(descriptor.ClientId, nameof(descriptor.ClientId));
        
        var application = await manager.FindByClientIdAsync(descriptor.ClientId, cancellationToken);

        if (application is not null)
        {
            await manager.UpdateAsync(application, descriptor, cancellationToken);
        }
        else
        {
            await manager.CreateAsync(descriptor, cancellationToken);
        }
        
        return manager;
    }
}
using System.Diagnostics.CodeAnalysis;
using OpenIddict.Abstractions;

namespace Nixon.Identity.OpenIddict.Extensions;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "ConvertToExtensionBlock")]
public static class OpenIddictApplicationDescriptorExtensions
{
    public static OpenIddictApplicationDescriptor AddRedirectUris(
        this OpenIddictApplicationDescriptor descriptor,
        IEnumerable<Uri> redirectUris)
    {
        foreach (var uri in redirectUris)
        {
            descriptor.RedirectUris.Add(uri);
        }
        
        return descriptor;
    }
    
    public static OpenIddictApplicationDescriptor AddRedirectUris(
        this OpenIddictApplicationDescriptor descriptor,
        IEnumerable<string> redirectUris)
    {
        foreach (var uri in redirectUris)
        {
            descriptor.RedirectUris.Add( new Uri(uri) );
        }
        
        return descriptor;
    }
    
}
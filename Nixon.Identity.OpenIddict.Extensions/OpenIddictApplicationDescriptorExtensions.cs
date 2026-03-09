using System.Diagnostics.CodeAnalysis;
using OpenIddict.Abstractions;

namespace Nixon.Identity.OpenIddict.Extensions;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "ConvertToExtensionBlock")]
public static class OpenIddictApplicationDescriptorExtensions
{
    public static OpenIddictApplicationDescriptor AddRedirectUris(
        this OpenIddictApplicationDescriptor descriptopr,
        IEnumerable<Uri> redirectUris)
    {
        foreach (var uri in redirectUris)
        {
            descriptopr.RedirectUris.Add(uri);
        }
        
        return descriptopr;
    }
    
    public static OpenIddictApplicationDescriptor AddRedirectUris(
        this OpenIddictApplicationDescriptor descriptopr,
        IEnumerable<string> redirectUris)
    {
        foreach (var uri in redirectUris)
        {
            descriptopr.RedirectUris.Add( new Uri(uri) );
        }
        
        return descriptopr;
    }
    
}
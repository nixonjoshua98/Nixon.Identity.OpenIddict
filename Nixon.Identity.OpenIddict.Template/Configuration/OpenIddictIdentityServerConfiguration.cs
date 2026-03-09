using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Nixon.Identity.OpenIddict.Template.Configuration;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
internal sealed class OpenIddictIdentityServerConfiguration
{
    public string Issuer { get; init; } = null!;
    public string EncryptionKey { get; init; } = null!;
    
    public string ClientId { get; init; } = null!;
    public string ClientSecret { get; init; } = null!;
    
    public string[] AllowedCustomGrantTypes { get; init; } = [];

    public SecurityKey EncryptionSecurityKey => 
        field ??= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(EncryptionKey));

    public static OpenIddictIdentityServerConfiguration ReadFrom(IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection("OpenIddictIdentityServer");

        var loaded = new OpenIddictIdentityServerConfiguration();
        
        section.Bind(loaded);
        
        return loaded;
    }
}
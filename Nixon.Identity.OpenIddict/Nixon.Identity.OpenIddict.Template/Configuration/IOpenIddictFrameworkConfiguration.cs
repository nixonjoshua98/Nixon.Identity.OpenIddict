using Microsoft.IdentityModel.Tokens;

namespace Nixon.Identity.OpenIddict.Template.Configuration;

public interface IOpenIddictFrameworkConfiguration
{
    string Issuer { get;  }

    IEnumerable<string> AllAllowedGrantTypes  { get;  }

    IOpenIddictFrameworkApplicationConfiguration[] Applications { get;  } 

    SecurityKey EncryptionSecurityKey { get;  }

    IEnumerable<string> GetRedirectUris(IOpenIddictFrameworkApplicationConfiguration application);
}
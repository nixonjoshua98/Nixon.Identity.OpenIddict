using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Server;

namespace Nixon.Identity.OpenIddict.Template.Builders;

public sealed class OpenIdDictIdentityServerBuilder
{
    public readonly ServerBuilder Server = new();
}

public sealed class ServerBuilder
{ 
    internal Action<OpenIddictServerBuilder>? ConfigureAction = null; 
    
    public ServerBuilder AddScopedTokenRequestHandler<THandler>() 
        where THandler : class, IOpenIddictServerHandler<OpenIddictServerEvents.HandleTokenRequestContext>
    {
        ConfigureAction += builder => builder
            .AddEventHandler<OpenIddictServerEvents.HandleTokenRequestContext>(x => x.UseScopedHandler<THandler>());
        
        return this;
      }
}
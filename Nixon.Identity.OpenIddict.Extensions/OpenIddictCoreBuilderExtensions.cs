using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.EntityFrameworkCore.Models;

namespace Nixon.Identity.OpenIddict.Extensions;

[SuppressMessage("ReSharper", "ConvertToExtensionBlock")]
public static class OpenIddictCoreBuilderExtensions
{
    public static OpenIddictEntityFrameworkCoreBuilder UseEntityFrameworkCore<TContext>(this OpenIddictCoreBuilder builder)
        where TContext : DbContext
    {
        return builder
            .UseEntityFrameworkCore()
            .UseDbContext<TContext>();
    }
    
    public static OpenIddictEntityFrameworkCoreBuilder UseEntityFrameworkCore<TContext, TApplication, TAuthorization, TScope, TToken, TKey>(this OpenIddictCoreBuilder builder)
        where TContext : DbContext
        where TApplication : OpenIddictEntityFrameworkCoreApplication<TKey, TAuthorization, TToken>
        where TAuthorization : OpenIddictEntityFrameworkCoreAuthorization<TKey, TApplication, TToken>
        where TScope : OpenIddictEntityFrameworkCoreScope<TKey>
        where TToken : OpenIddictEntityFrameworkCoreToken<TKey, TApplication, TAuthorization>
        where TKey : IEquatable<TKey>
    {
        return builder
            .UseEntityFrameworkCore()
            .UseDbContext<TContext>()
            .ReplaceDefaultEntities<TApplication, TAuthorization, TScope, TToken, TKey>();
    }
}
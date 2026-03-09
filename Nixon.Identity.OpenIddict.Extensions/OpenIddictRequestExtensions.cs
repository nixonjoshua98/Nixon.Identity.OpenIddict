using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using OpenIddict.Abstractions;

namespace Nixon.Identity.OpenIddict.Extensions;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "ConvertToExtensionBlock")]
public static class OpenIddictRequestExtensions
{
    public static bool TryGetParameter<T>(this OpenIddictRequest request, string name, [NotNullWhen(true)] out T? value) 
        where T : IParsable<T>
    {
        return TryGetParameter(request, name, CultureInfo.InvariantCulture, out value);
    }

    public static bool TryGetParameter<T>(this OpenIddictRequest request, string name, IFormatProvider provider, [NotNullWhen(true)] out T? value) 
        where T : IParsable<T>
    {
        value = default;

        return TryGetParameter(request, name, out var str) && T.TryParse(str, provider, out value);
    }

    public static bool TryGetParameter(this OpenIddictRequest request, string name, [NotNullWhen(true)] out string? value)
    {
        value = null;

        if (!request.TryGetParameter(name, out var param))
        {
            return false;
        }

        value = param.ToString();

        return !string.IsNullOrEmpty(value);
    }
}
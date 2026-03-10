using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace Nixon.Extensions.Configuration;

[SuppressMessage("ReSharper", "ConvertToExtensionBlock")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class ConfigurationExtensions
{
    public static T GetRequiredSection<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        T instance = new();

        var section = configuration.GetRequiredSection(sectionName);

        section.Bind(instance);

        return instance;
    }
    
    public static T GetValueOrDefault<T>(this IConfiguration configuration, string key, T defaultValue = default!) where T : IParsable<T>
    {
        var value = configuration[key];

        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        if (T.TryParse(value, CultureInfo.InvariantCulture, out var parsedValue))
        {
            return parsedValue;
        }

        throw new Exception($"Configuration key '{key}' was found but failed to be parsed");
    }
    
    public static string GetRequiredConnectionString(this IConfiguration configuration, string key) =>
        GetRequiredValue(configuration, $"ConnectionStrings:{key}");
    
    public static string GetRequiredValue(this IConfiguration configuration, string key) =>
        configuration[key] ?? throw new Exception($"Configuration key missing '{key}'");
}
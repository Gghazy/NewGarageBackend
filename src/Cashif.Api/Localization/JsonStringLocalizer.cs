namespace Cashif.Api.Localization;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Localization;


public class JsonStringLocalizer : IStringLocalizer
{
    private readonly string _resourcesPath;
    private readonly ConcurrentDictionary<string, Dictionary<string, string>> _cache = new();

    public JsonStringLocalizer(string resourcesPath) => _resourcesPath = resourcesPath;

    private Dictionary<string, string> LoadCulture(string culture)
    {
        return _cache.GetOrAdd(culture, c =>
        {
            var file = Path.Combine(_resourcesPath, $"{c}.json");
            if (!File.Exists(file))
            {
                var neutral = c.Split('-')[0];
                file = Path.Combine(_resourcesPath, $"{neutral}.json");
            }
            if (!File.Exists(file)) return new Dictionary<string, string>();

            var json = File.ReadAllText(file);

            using var doc = JsonDocument.Parse(json);
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            FlattenJson(doc.RootElement, prefix: "", result);

            return result;
        });
    }

    private static void FlattenJson(JsonElement element, string prefix, Dictionary<string, string> result)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var prop in element.EnumerateObject())
                {
                    var key = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";
                    FlattenJson(prop.Value, key, result);
                }
                break;

            case JsonValueKind.Array:
                int i = 0;
                foreach (var item in element.EnumerateArray())
                {
                    var key = $"{prefix}[{i++}]";
                    FlattenJson(item, key, result);
                }
                break;

            default:
                // string/number/bool/null => نحولها string
                result[prefix] = element.ToString() ?? "";
                break;
        }
    }


    public LocalizedString this[string name]
    {
        get
        {
            var culture = CultureInfo.CurrentUICulture.Name;
            var dict = LoadCulture(culture);
            var found = dict.TryGetValue(name, out var value);
            return new LocalizedString(name, found ? value! : name, !found);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var raw = this[name];
            var formatted = string.Format(CultureInfo.CurrentCulture, raw.Value, arguments);
            return new LocalizedString(name, formatted, raw.ResourceNotFound);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var culture = CultureInfo.CurrentUICulture.Name;
        var dict = LoadCulture(culture);
        foreach (var kv in dict)
            yield return new LocalizedString(kv.Key, kv.Value, false);
    }

    public IStringLocalizer WithCulture(CultureInfo culture) => this;
}

public class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly string _resourcesPath;

    public JsonStringLocalizerFactory(IWebHostEnvironment env, IConfiguration cfg)
    {
        // المسار الافتراضي: Localization/
        var folder = cfg["Localization:Folder"] ?? Path.Combine("Localization", "Resources");
        _resourcesPath = Path.Combine(env.ContentRootPath, folder);
        Directory.CreateDirectory(_resourcesPath);
    }

    public IStringLocalizer Create(Type resourceSource) => new JsonStringLocalizer(_resourcesPath);
    public IStringLocalizer Create(string baseName, string location) => new JsonStringLocalizer(_resourcesPath);
}

public static class JsonLocalizationSetup
{
    public static IServiceCollection AddJsonLocalization(this IServiceCollection services, string? resourcesFolder = null)
    {
        services.AddLocalization();

        services.AddSingleton<IStringLocalizerFactory>(sp =>
        {
            var env = sp.GetRequiredService<IWebHostEnvironment>();
            var cfg = sp.GetRequiredService<IConfiguration>();
            if (!string.IsNullOrWhiteSpace(resourcesFolder))
            {
                var memCfg = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Localization:Folder"] = resourcesFolder
                    })
                    .Build();

                return new JsonStringLocalizerFactory(env, memCfg);
            }

            return new JsonStringLocalizerFactory(env, cfg);
        });

        services.AddSingleton<IStringLocalizer>(sp =>
            sp.GetRequiredService<IStringLocalizerFactory>().Create("Shared", ""));

        return services;
    }


}





